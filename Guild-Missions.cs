using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
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

        private const int MAX_RESULT_COUNT = 7;
        
        private Panel trekListPanel, savedTrekListPanel, contentPanel, listPanel, infoPanel;
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

        private Texture2D _guildMissionIcon;
        private Texture2D _guildTrekIcon;
        private Texture2D _guildBountyIcon;
        private Texture2D _guildRaceIcon;
        private Texture2D _guildPuzzleIcon;
        private Texture2D _guildChallengeIcon;

        private Texture2D _lockedIcon;
        private Texture2D _wipIcon;

        private Texture2D _waypointIcon;
        private Texture2D _rightArrowIcon;

        private Texture2D _closeTexture;

        internal string GuildTrekTabName = "Guild Missions";

        private WindowTab _moduleTab;
        private TextBox searchTextBox;

        Dictionary<int, Texture2D> _guildRaceMap = new Dictionary<int, Texture2D>();
        Dictionary<int, Texture2D> _guildPuzzleInfo = new Dictionary<int, Texture2D>();
        Dictionary<int, Texture2D> _guildBountyInfo = new Dictionary<int, Texture2D>();
        Dictionary<int, Texture2D> _guildChallengeInfo = new Dictionary<int, Texture2D>();

        private int panelsize = 56;

        protected override void Initialize()
        {
            _guildMissionIcon = ContentsManager.GetTexture("528697.png");
            _guildTrekIcon = ContentsManager.GetTexture("1228320.png");
            _guildBountyIcon = ContentsManager.GetTexture("1228316.png");
            _guildRaceIcon = ContentsManager.GetTexture("1228319.png");
            _guildPuzzleIcon = ContentsManager.GetTexture("1228318.png");
            _guildChallengeIcon = ContentsManager.GetTexture("1228317.png");

            _lockedIcon = ContentsManager.GetTexture("1827421.png");
            _wipIcon = ContentsManager.GetTexture("2221493.png");

            _waypointIcon = ContentsManager.GetTexture("157354.png");
            _rightArrowIcon = ContentsManager.GetTexture("784266.png");

            _closeTexture = ContentsManager.GetTexture("close_icon.png");

            _guildPuzzleInfo.Add(1, ContentsManager.GetTexture("1827421.png"));
            _guildPuzzleInfo.Add(2, ContentsManager.GetTexture("1827421.png"));
            _guildPuzzleInfo.Add(3, ContentsManager.GetTexture("1827421.png"));

            _guildRaceMap.Add(1, ContentsManager.GetTexture("racemaps/bear_lope.jpg"));
            _guildRaceMap.Add(2, ContentsManager.GetTexture("racemaps/chicken_run.jpg"));
            _guildRaceMap.Add(3, ContentsManager.GetTexture("racemaps/crab_scuttle.jpg"));
            _guildRaceMap.Add(4, ContentsManager.GetTexture("racemaps/devourer_burrow.jpg"));
            _guildRaceMap.Add(5, ContentsManager.GetTexture("racemaps/ghost_wolf_run.jpg"));
            _guildRaceMap.Add(6, ContentsManager.GetTexture("racemaps/quaggan_paddle.jpg"));
            _guildRaceMap.Add(7, ContentsManager.GetTexture("racemaps/spider_scurry.jpg"));

            _guildBountyInfo.Add(1, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(2, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(3, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(4, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(5, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(6, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(7, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(8, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(9, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(10, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(11, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(12, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(13, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(14, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(15, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(16, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(17, ContentsManager.GetTexture("1827421.png"));
            _guildBountyInfo.Add(18, ContentsManager.GetTexture("1827421.png"));

            _guildChallengeInfo.Add(1, ContentsManager.GetTexture("1827421.png"));
            _guildChallengeInfo.Add(2, ContentsManager.GetTexture("1827421.png"));
            _guildChallengeInfo.Add(3, ContentsManager.GetTexture("1827421.png"));
            _guildChallengeInfo.Add(4, ContentsManager.GetTexture("1827421.png"));
            _guildChallengeInfo.Add(5, ContentsManager.GetTexture("1827421.png"));
            _guildChallengeInfo.Add(6, ContentsManager.GetTexture("1827421.png"));
        }

        protected override async Task LoadAsync()
        {

        }

        protected override void OnModuleLoaded(EventArgs e)
        {

            _moduleTab = Overlay.BlishHudWindow.AddTab(GuildTrekTabName, _guildMissionIcon, GuildMissionsView(Overlay.BlishHudWindow));

            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        private Panel GuildMissionsView(WindowBase wndw)
        {
            var parentPanel = new Panel()
            {
                CanScroll = false,
                Size = wndw.ContentRegion.Size
            };

            var missionTypePanel = new Panel()
            {
                ShowBorder = true,
                Title = "Choose Guild Mission Type",
                Size = new Point(265, parentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(LEFT_MARGIN, TOP_MARGIN),
                Parent = parentPanel,
            };

            var guildTrekPanel = new Panel()
            {
                ShowBorder = false,
                Size = new Point(missionTypePanel.Width, panelsize),
                Location = new Point(0, 0),
                Parent = missionTypePanel,
            };
            guildTrekPanel.Click += delegate { guildTrekContent(); };
            new Image(_guildTrekIcon)
            {
                Size = new Point(panelsize, panelsize),
                Location = new Point(0, 0),
                Parent = guildTrekPanel
            };
            new Label()
            {
                Text = "Trek",
                Font = Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN + panelsize, panelsize / 2 - 10),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = guildTrekPanel
            };

            var guildBountyPanel = new Panel()
            {
                ShowBorder = false,
                Size = new Point(missionTypePanel.Width, panelsize),
                Location = new Point(0, panelsize),
                Parent = missionTypePanel,
            };
            guildBountyPanel.Click += delegate { guildBountyContent(); };
            new Image(_guildBountyIcon)
            {
                Size = new Point(panelsize, panelsize),
                Location = new Point(0, 0),
                Parent = guildBountyPanel
            };
            new Label()
            {
                Text = "Bounty",
                Font = Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN + panelsize, panelsize / 2 - 10),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = guildBountyPanel
            };

            var guildRacePanel = new Panel()
            {
                ShowBorder = false,
                Size = new Point(missionTypePanel.Width, panelsize),
                Location = new Point(0, panelsize * 2),
                Parent = missionTypePanel,
            };
            guildRacePanel.Click += delegate { guildRaceContent(); };
            new Image(_guildRaceIcon)
            {
                Size = new Point(panelsize, panelsize),
                Location = new Point(0, 0),
                Parent = guildRacePanel
            };
            new Label()
            {
                Text = "Race",
                Font = Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN + panelsize, panelsize / 2 - 10),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = guildRacePanel
            };

            var guildChallengePanel = new Panel()
            {
                ShowBorder = false,
                Size = new Point(missionTypePanel.Width, panelsize),
                Location = new Point(0, panelsize * 3),
                Parent = missionTypePanel,
            };
            guildChallengePanel.Click += delegate { guildChallengeContent(); };
            new Image(_guildChallengeIcon)
            {
                Size = new Point(panelsize, panelsize),
                Location = new Point(0, 0),
                Parent = guildChallengePanel
            };
            new Label()
            {
                Text = "Challenge",
                Font = Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN + panelsize, panelsize / 2 - 10),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = guildChallengePanel
            };

            var guildPuzzlePanel = new Panel()
            {
                ShowBorder = false,
                Size = new Point(missionTypePanel.Width, panelsize),
                Location = new Point(0, panelsize * 4),
                Parent = missionTypePanel,
            };
            guildPuzzlePanel.Click += delegate { guildPuzzleContent(); };
            new Image(_guildPuzzleIcon)
            {
                Size = new Point(panelsize, panelsize),
                Location = new Point(0, 0),
                Parent = guildPuzzlePanel
            };
            new Label()
            {
                Text = "Puzzle",
                Font = Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN + panelsize, panelsize / 2 - 10),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = guildPuzzlePanel
            };


            contentPanel = new Panel()
            {
                ShowBorder = false,
                Size = new Point(parentPanel.Width - missionTypePanel.Right - RIGHT_MARGIN, parentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(missionTypePanel.Right + LEFT_MARGIN, TOP_MARGIN),
                Parent = parentPanel,
            };

            return parentPanel;
        }

        private void guildTrekContent()
        {
            contentPanel.ClearChildren();

            new Image(_guildTrekIcon)
            {
                Size = new Point(72, 72),
                Location = new Point(LEFT_MARGIN, 0),
                Parent = contentPanel
            };
            new Label()
            {
                Text = "Trek",
                Font = Content.DefaultFont32,
                Location = new Point(82, 18),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = contentPanel
            };

            searchTextBox = new TextBox()
            {
                PlaceholderText = "Enter name here ...",
                Size = new Point(358, 43),
                Font = GameService.Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };
            searchTextBox.TextChanged += SearchboxOnTextChanged;

            trekListPanel = new Panel()
            {
                ShowBorder = true,
                Title = "Search Results",
                Size = new Point(364, contentPanel.Height - searchTextBox.Bottom - BOTTOM_MARGIN),
                Location = new Point(LEFT_MARGIN - 3, searchTextBox.Bottom + TOP_MARGIN),
                Parent = contentPanel,
            };

            savedTrekListPanel = new Panel()
            {
                CanScroll = true,
                ShowBorder = true,
                Title = "Saved Treks",
                Size = new Point(364, contentPanel.Height - searchTextBox.Bottom - BOTTOM_MARGIN),
                Location = new Point(trekListPanel.Right + LEFT_MARGIN, searchTextBox.Bottom + TOP_MARGIN),
                Parent = contentPanel,
            };

            var clearAllButton = new StandardButton()
            {
                Text = "Clear All",
                Size = new Point(110, 30),
                Location = new Point(trekListPanel.Right + 20, searchTextBox.Top - 1),
                Parent = contentPanel,
            };
            clearAllButton.Click += delegate { ClearWPList(); };

            var exportButton = new StandardButton()
            {
                Text = "Export",
                Size = new Point(110, 30),
                Location = new Point(trekListPanel.Right + 130 + LEFT_MARGIN, searchTextBox.Top - 1),
                Parent = contentPanel,
            };
            exportButton.Click += delegate { ExportWPList(); };

            var importButton = new StandardButton()
            {
                Text = "Import",
                Size = new Point(110, 30),
                Location = new Point(trekListPanel.Right + 250 + LEFT_MARGIN, searchTextBox.Top - 1),
                Parent = contentPanel,
            };
            importButton.Click += delegate { ImportWPList(); };

            UpdateSavedWPList();
        }

        private void guildRaceContent()
        {
            contentPanel.ClearChildren();

            new Image(_guildRaceIcon)
            {
                Size = new Point(72, 72),
                Location = new Point(LEFT_MARGIN, 0),
                Parent = contentPanel
            };
            new Label()
            {
                Text = "Race",
                Font = Content.DefaultFont32,
                Location = new Point(82, 18),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = contentPanel
            };

            listPanel = new Panel()
            {
                ShowBorder = true,
                Title = "List",
                Size = new Point(364, contentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(LEFT_MARGIN - 3, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };

            infoPanel = new Panel()
            {
                CanScroll = true,
                ShowBorder = true,
                Title = "Info",
                Size = new Point(364, contentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(listPanel.Right + LEFT_MARGIN, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };

            // Dispose of current search result
            listPanel.ClearChildren();

            XDocument doc = XDocument.Load(ContentsManager.GetFileStream("guildrace_data.xml"));

            int i = 0;
            foreach (var race in doc.Root.Elements("race"))
            {
                ViewInfoPanel(race, listPanel, i, "race");
                i++;
            }
        }

        private void guildBountyContent()
        {
            contentPanel.ClearChildren();

            new Image(_guildBountyIcon)
            {
                Size = new Point(72, 72),
                Location = new Point(LEFT_MARGIN, 0),
                Parent = contentPanel
            };
            new Label()
            {
                Text = "Bounty",
                Font = Content.DefaultFont32,
                Location = new Point(82, 18),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = contentPanel
            };

            listPanel = new Panel()
            {
                ShowBorder = true,
                Title = "List",
                Size = new Point(364, contentPanel.Height - BOTTOM_MARGIN - 72),
                Location = new Point(LEFT_MARGIN - 3, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };

            infoPanel = new Panel()
            {
                CanScroll = true,
                ShowBorder = true,
                Title = "Info",
                Size = new Point(364, contentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(listPanel.Right + LEFT_MARGIN, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };

            // Dispose of current search result
            listPanel.ClearChildren();

            XDocument doc = XDocument.Load(ContentsManager.GetFileStream("guildbounty_data.xml"));

            int i = 0;
            foreach (var bounty in doc.Root.Elements("bounty"))
            {
                ViewInfoPanel(bounty, listPanel, i, "bounty", 20);
                i++;
            }

            listPanel.CanScroll = true;
        }

        private void guildChallengeContent()
        {
            contentPanel.ClearChildren();

            new Image(_guildChallengeIcon)
            {
                Size = new Point(72, 72),
                Location = new Point(LEFT_MARGIN, 0),
                Parent = contentPanel
            };
            new Label()
            {
                Text = "Challenge",
                Font = Content.DefaultFont32,
                Location = new Point(82, 18),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = contentPanel
            };

            listPanel = new Panel()
            {
                ShowBorder = true,
                Title = "List",
                Size = new Point(364, contentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(LEFT_MARGIN - 3, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };

            infoPanel = new Panel()
            {
                CanScroll = true,
                ShowBorder = true,
                Title = "Info",
                Size = new Point(364, contentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(listPanel.Right + LEFT_MARGIN, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };

            // Dispose of current search result
            listPanel.ClearChildren();

            XDocument doc = XDocument.Load(ContentsManager.GetFileStream("guildchallenge_data.xml"));

            int i = 0;
            foreach (var challenge in doc.Root.Elements("challenge"))
            {
                ViewInfoPanel(challenge, listPanel, i, "challenge");
                i++;
            }
        }

        private void guildPuzzleContent()
        {
            contentPanel.ClearChildren();

            new Image(_guildPuzzleIcon)
            {
                Size = new Point(72, 72),
                Location = new Point(LEFT_MARGIN, 0),
                Parent = contentPanel
            };
            new Label()
            {
                Text = "Puzzle",
                Font = Content.DefaultFont32,
                Location = new Point(82, 18),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = contentPanel
            };

            listPanel = new Panel()
            {
                ShowBorder = true,
                Title = "List",
                Size = new Point(364, contentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(LEFT_MARGIN - 3, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };

            infoPanel = new Panel()
            {
                CanScroll = true,
                ShowBorder = true,
                Title = "Info",
                Size = new Point(364, contentPanel.Height - BOTTOM_MARGIN),
                Location = new Point(listPanel.Right + LEFT_MARGIN, 72 + TOP_MARGIN),
                Parent = contentPanel,
            };

            // Dispose of current search result
            listPanel.ClearChildren();

            XDocument doc = XDocument.Load(ContentsManager.GetFileStream("guildpuzzle_data.xml"));

            int i = 0;
            foreach (var puzzle in doc.Root.Elements("puzzle"))
            {
                ViewInfoPanel(puzzle, listPanel, i, "puzzle");
                i++;
            }
        }

        private void lockedContent()
        {
            contentPanel.ClearChildren();

            new Image(_lockedIcon)
            {
                Size = new Point(656, 680),
                Location = new Point(contentPanel.Width / 2 - 328, contentPanel.Height / 2 - 340),
                Parent = contentPanel
            };
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
                if (trek.Element("Name").Value.ToLower().StartsWith(searchText.ToLower()))
                //if (trek.Element("Name").Value.ToLower().Contains(searchText.ToLower()))
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

        private void ExportWPList()
        {
            int i = 0;
            var export = "BlishGM";
            foreach (KeyValuePair<int, int> wp in savedGuildTreks.OrderBy(key => key.Value))
            {
                export = export + ';' + wp.Key.ToString();
            }

            ClipboardUtil.WindowsClipboardService.SetTextAsync(export).ContinueWith((clipboardResult) =>
            {
                if (clipboardResult.IsFaulted)
                {
                    ScreenNotification.ShowNotification("Failed to copy export to clipboard. Try again.", ScreenNotification.NotificationType.Red, duration: 2);
                }
                else
                {
                    ScreenNotification.ShowNotification("Copied export to clipboard!", duration: 2);
                }
            });
        }

        private void ImportWPList()
        {
            XDocument doc = XDocument.Load(ContentsManager.GetFileStream("guildtrek_data.xml"));
            ClipboardUtil.WindowsClipboardService.GetTextAsync()
                .ContinueWith((import) => {
                    if (!import.IsFaulted)
                    {
                        if (!string.IsNullOrEmpty(import.Result))
                        {
                            int i = 0;
                            foreach (string wp in import.Result.Split(';'))
                            {
                                if (i == 0 && String.Equals(wp, "BlishGM"))
                                {
                                    i++;
                                    continue;
                                }
                                else if (i == 0 && !String.Equals(wp, "BlishGM")) return;

                                Logger.Warn(import.Exception, i + ":" + wp);

                                // Grab trek data from xml
                                var trek = doc.Descendants("trek")
                                    .Where(x => x.Element("ID").Value == wp)
                                    .FirstOrDefault();

                                if (trek == null) continue;

                                AddWPToList((int)trek.Element("ID"), (int)trek.Element("MapID"));
                                i++;
                            }

                            ScreenNotification.ShowNotification("Imported " + (i-1) + " waypoints from clipboard!", duration: 2);
                        }
                    }
                    else
                    {
                        Logger.Warn(import.Exception, "Failed to read clipboard text from system clipboard!");
                    }
                });
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
                    .Where(x => x.Element("ID").Value == wp.Key.ToString())
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
                //Title = trek.Element("Name").Value + " (" + trek.Element("MapName").Value + ")",
                Size = new Point(parent.Width, 70),
                Location = new Point(LEFT_MARGIN, 5 + position * 70),
                Parent = parent
            };
            Image trekPanelWPImage = new Image(_waypointIcon)
            {
                Size = new Point(50, 50),
                Location = new Point(0, 4),
                Parent = trekPanel
            };
            trekPanelWPImage.Click += delegate
            {
                ClipboardUtil.WindowsClipboardService.SetTextAsync(trek.Element("Name").Value + " " + trek.Element("WaypointChatcode").Value).ContinueWith((clipboardResult) =>
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
                Text = trek.Element("Name").Value + " (" + trek.Element("MapName").Value + ")",
                Font = Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN + 50, 3),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = trekPanel
            };
            new Label()
            {
                Text = trek.Element("WaypointName").Value,
                Font = Content.DefaultFont14,
                Location = new Point(LEFT_MARGIN + 50, 32),
                TextColor = Color.Silver,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = trekPanel
            };

            if (add)
            {
                Image addImage = new Image(_rightArrowIcon)
                {
                    Size = new Point(70, 70),
                    Location = new Point(parent.Width - 70, -10),
                    Parent = trekPanel
                };
                addImage.Click += delegate { AddWPToList((int)trek.Element("ID"), (int)trek.Element("MapID")); };
            }

            if (remove)
            {
                Image removeImage = new Image(_closeTexture)
                {
                    Size = new Point(20, 20),
                    Location = new Point(parent.Width - 40, 4),
                    Parent = trekPanel
                };
                removeImage.Click += delegate { RemoveWPFromList((int)trek.Element("ID")); };
            }
        }

        private void ViewInfoPanel(XElement element, Panel parent, int position, String type, int offset = 0)
        {
            Panel trekPanel = new Panel()
            {
                ShowBorder = false,
                //Title = trek.Element("Name").Value + " (" + trek.Element("MapName").Value + ")",
                Size = new Point(parent.Width, 70),
                Location = new Point(LEFT_MARGIN, 5 + position * 70),
                Parent = parent
            };
            Image trekPanelWPImage = new Image(_waypointIcon)
            {
                Size = new Point(50, 50),
                Location = new Point(0, 4),
                Parent = trekPanel
            };
            trekPanelWPImage.Click += delegate
            {
                ClipboardUtil.WindowsClipboardService.SetTextAsync(element.Element("Name").Value + " " + element.Element("WaypointChatcode").Value).ContinueWith((clipboardResult) =>
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
                Text = element.Element("Name").Value + " (" + element.Element("MapName").Value + ")",
                Font = Content.DefaultFont16,
                Location = new Point(LEFT_MARGIN + 50, 3),
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = trekPanel
            };
            new Label()
            {
                Text = element.Element("WaypointName").Value,
                Font = Content.DefaultFont14,
                Location = new Point(LEFT_MARGIN + 50, 32),
                TextColor = Color.Silver,
                ShadowColor = Color.Black,
                ShowShadow = true,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = trekPanel
            };
            Image addImage = new Image(_rightArrowIcon)
            {
                Size = new Point(70, 70),
                Location = new Point(parent.Width - 70 - offset, -10),
                Parent = trekPanel
            };
            addImage.Click += delegate { DisplayInfo((int)element.Element("ID"), type, element); };
        }

        private void DisplayInfo(int v, String type, XElement element)
        {
            int offset = 0;

            infoPanel.ClearChildren();
            infoPanel.Title = "Info: " + element.Element("Name").Value;

            if (element.Element("Wiki") != null)
            {
                var openWikiBttn = new StandardButton()
                {
                    Text = "Open Wiki",
                    Size = new Point(110, 30),
                    Location = new Point(4, 4),
                    Parent = infoPanel,
                };
                openWikiBttn.Click += delegate { Process.Start(element.Element("Wiki").Value); };
                offset += 40;
            }

            switch (type)
            {
                case "race":
                    new Image(_guildRaceMap[v])
                    {
                        Size = new Point(310, 500),
                        Location = new Point(4, 4 + offset),
                        Parent = infoPanel
                    };
                    break;
                case "puzzle":
                    new Image(_guildPuzzleInfo[v])
                    {
                        Size = new Point(310, 200),
                        Location = new Point(4, 4 + offset),
                        Parent = infoPanel
                    };
                    break;
                case "challenge":
                    new Image(_guildChallengeInfo[v])
                    {
                        Size = new Point(310, 200),
                        Location = new Point(4, 4 + offset),
                        Parent = infoPanel
                    };
                    break;
                default:
                    break;
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
