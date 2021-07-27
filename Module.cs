using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Blish_HUD.GameService;

namespace Guild_Module
{

    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        internal static Module ModuleInstance;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        protected override void DefineSettings(SettingCollection settings)
        {

        }

        private Texture2D _guildTrekIconTexture;

        internal string GuildTrekTabName = "Guild Trek";

        private WindowTab _moduleTab;

        protected override void Initialize()
        {
            _guildTrekIconTexture = ContentsManager.GetTexture("guildtrek_icon.png");
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
            var hPanel = new Panel()
            {
                CanScroll = false,
                Size = wndw.ContentRegion.Size
            };

            return hPanel;
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
