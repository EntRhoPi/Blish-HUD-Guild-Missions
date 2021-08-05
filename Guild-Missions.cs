using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Blish_HUD.GameService;

namespace entrhopi.Guild_Missions
{

    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Guild_Missions : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        internal static Module ModuleInstance;
        #region Constants

        private const int TOP_MARGIN = 10;
        private const int RIGHT_MARGIN = 5;
        private const int BOTTOM_MARGIN = 10;
        private const int LEFT_MARGIN = 9;

        private const int MAX_RESULT_COUNT = 8;
        
        private Panel trekListPanel, savedTrekListPanel;
        public List<Panel> resultPanels = new List<Panel>();
        Dictionary<int, int> savedGuildTreks = new Dictionary<int, int>();


        #endregion

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public Guild_Missions([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        protected override void DefineSettings(SettingCollection settings)
        {

        }

        private Texture2D _guildTrekIconTexture;
        private Texture2D _arrowRightIconTexture;
        private Texture2D _wpTexture;
        private Texture2D _closeTexture;

        internal string GuildTrekTabName = "Guild Trek";

        private WindowTab _moduleTab;
        private TextBox searchTextBox;

        protected override void Initialize()
        {
            _guildTrekIconTexture = ContentsManager.GetTexture("guildtrek_icon.png");
            _arrowRightIconTexture = ContentsManager.GetTexture("arrow_right_icon.png");
            _wpTexture = ContentsManager.GetTexture("157353.png");
            _closeTexture = ContentsManager.GetTexture("close_icon.png");
        }

        protected override async Task LoadAsync()
        {

        }

        protected override void OnModuleLoaded(EventArgs e)
        {

            _moduleTab = Overlay.BlishHudWindow.AddTab(GuildTrekTabName, _guildTrekIconTexture, GuildTrekView(Overlay.BlishHudWindow));

            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        private Panel GuildTrekView(WindowBase wndw)
        {
            var parentPanel = new Panel()
            {
                CanScroll = false,
                Size = wndw.ContentRegion.Size
            };

            Image clearAllImage = new Image(_closeTexture)
            {
                Size = new Point(50, 50),
                Location = new Point(parentPanel.Width / 2 - 25, parentPanel.Height / 2 - 25),
                Parent = parentPanel
            };
            clearAllImage.Click += delegate { ClearWPList(); };

            searchTextBox = new TextBox()
            {
                PlaceholderText = "Enter name here ...",
                Size = new Point(parentPanel.Width - LEFT_MARGIN - RIGHT_MARGIN, 43),
                Font = GameService.Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN, TOP_MARGIN),
                Parent = parentPanel,
            };

            trekListPanel = new Panel()
            {
                ShowBorder = true,
                Title = "Search Guild Treks",
                Size = new Point(400, parentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(LEFT_MARGIN, searchTextBox.Bottom + TOP_MARGIN),
                Parent = parentPanel,
            };


            savedTrekListPanel = new Panel()
            {
                CanScroll = true,
                ShowBorder = true,
                Title = "Saved Guild Treks",
                Size = new Point(400, parentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(parentPanel.Width - 400, searchTextBox.Bottom + TOP_MARGIN),
                Parent = parentPanel,
            };

            searchTextBox.TextChanged += SearchboxOnTextChanged;

            return parentPanel;
        }

        private void SearchboxOnTextChanged(object sender, EventArgs e)
        {
            int i = 0;

            // Load user input
            string searchText = searchTextBox.Text;

            // Dispose of current search result
            trekListPanel.ClearChildren();

            XDocument doc = XDocument.Load(ContentsManager.GetFileStream("guildtrek_data.xml"));

            foreach(var trek in doc.Root.Elements("trek"))
            {
                //if (trek.Element("TrekName").Value.ToLower().StartsWith(searchText.ToLower()))
                if (trek.Element("TrekName").Value.ToLower().Contains(searchText.ToLower()))
                {
                    AddTrekPanel(trek, trekListPanel, i, true, false);

                    i++;
                    if (i >= MAX_RESULT_COUNT) break;
                }
            }
        }

        private void AddWPToList(int trekID, int mapID)
        {
            if (savedGuildTreks.ContainsKey(trekID) == false)
            {
                savedGuildTreks.Add(trekID, mapID);
                UpdateSavedWPList();
            }

        }

        private void RemoveWPFromList(int trek)
        {
            savedGuildTreks.Remove(trek);
            UpdateSavedWPList();
        }

        private void ClearWPList()
        {
            savedGuildTreks.Clear();
            savedTrekListPanel.ClearChildren();
        }

        private void UpdateSavedWPList()
        {
            savedTrekListPanel.ClearChildren();

            XDocument doc = XDocument.Load(ContentsManager.GetFileStream("guildtrek_data.xml"));

            // Sort saved treks by map id
            int i = 0;
            foreach (KeyValuePair<int, int> wp in savedGuildTreks.OrderBy(key => key.Value))
            {
                // Grab trek data from xml
                var trek = doc.Descendants("trek")
                    .Where(x => x.Element("TrekID").Value == wp.Key.ToString())
                    .FirstOrDefault();

                if (trek == null) continue;

                AddTrekPanel(trek, savedTrekListPanel, i, false, true);

                i++;
            }
        }

        private void AddTrekPanel(XElement trek, Panel parent, int position, bool add = false, bool remove = false)
        {

            Panel trekPanel = new Panel()
            {
                ShowBorder = false,
                Title = trek.Element("TrekName").Value + " (" + trek.Element("MapName").Value + ")",
                Size = new Point(trekListPanel.Width, 70),
                Location = new Point(LEFT_MARGIN, 5 + position * 70),
                Parent = parent
            };
            Image trekPanelWPImage = new Image(_wpTexture)
            {
                Size = new Point(30, 30),
                Location = new Point(0, 0),
                Parent = trekPanel
            };
            trekPanelWPImage.Click += delegate
            {
                ClipboardUtil.WindowsClipboardService.SetTextAsync(trek.Element("TrekName").Value + " " + trek.Element("WaypointChatcode").Value).ContinueWith((clipboardResult) =>
                {
                    if (clipboardResult.IsFaulted)
                    {
                        ScreenNotification.ShowNotification("Failed to copy waypoint to clipboard. Try again.", ScreenNotification.NotificationType.Red, duration: 2);
                    }
                    else
                    {
                        ScreenNotification.ShowNotification("Copied waypoint to clipboard!", duration: 2);
                    }
                });
            };
            new Label()
            {
                Text = trek.Element("WaypointName").Value,
                Font = Content.DefaultFont14,
                Location = new Point(LEFT_MARGIN + 30, 5),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = trekPanel
            };

            if(add)
            {
                Image addImage = new Image(_arrowRightIconTexture)
                {
                    Size = new Point(20, 20),
                    Location = new Point(trekListPanel.Width - 40, 4),
                    Parent = trekPanel
                };
                addImage.Click += delegate { AddWPToList((int)trek.Element("TrekID"), (int)trek.Element("MapID")); };
            }

            if(remove)
            {
                Image removeImage = new Image(_closeTexture)
                {
                    Size = new Point(20, 20),
                    Location = new Point(trekListPanel.Width - 40, 4),
                    Parent = trekPanel
                };
                removeImage.Click += delegate { RemoveWPFromList((int)trek.Element("TrekID")); };
            }
        }

        protected override void Update(GameTime gameTime)
        {

        }

        /// <inheritdoc />
        protected override void Unload()
        {
            // Unload here
            Overlay.BlishHudWindow.RemoveTab(_moduleTab);
            // All static members must be manually unset
            ModuleInstance = null;
        }

    }

}
