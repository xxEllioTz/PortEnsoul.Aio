namespace xxEliot.Core
{
    #region

    using System.Windows.Forms;
    using EnsoulSharp.SDK.MenuUI.Values;


    using Menu = EnsoulSharp.SDK.MenuUI.Menu;

    #endregion

    internal static class Config
    {
        #region Static Fields

        private static int cBlank = -1;

        #endregion

        #region Public Methods and Operators

        public static MenuBool Bool(this Menu subMenu, string name, string display, bool state = true)
        {
            return subMenu.Add(new MenuBool(name, display, state));
        }

        public static MenuKeyBind KeyBind(
            this Menu subMenu,
            string name,
            string display,
            Keys key,
            KeyBindType type = KeyBindType.Press)
        {
            return subMenu.Add(new MenuKeyBind(name, display, key, type));
        }

        public static MenuList List(this Menu subMenu, string name, string display, string[] array, int value = 0)
        {
            return subMenu.Add(new MenuList(name, display, array) { Index = value });
        }

        public static MenuSeparator Separator(this Menu subMenu, string display)
        {
            cBlank += 1;
            return subMenu.Add(new MenuSeparator("xxEliot" + cBlank, display));
        }

        public static MenuSlider Slider(
            this Menu subMenu,
            string name,
            string display,
            int cur,
            int min = 0,
            int max = 100)
        {
            return subMenu.Add(new MenuSlider(name, display, cur, min, max));
        }

        #endregion
    }
}