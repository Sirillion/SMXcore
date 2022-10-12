//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: Laydor.

//	Changes hard coded sprites to be the sprites provided by the list.
//	Difference: Vanilla has these sprites hardcoded so it was impossible to add new sprites

namespace SMXcore
{
    public class XUiC_MapSubPopupList : XUiController
    {
        private static string[] sprites = new string[]
        {
            "ui_game_symbol_map_waypoint01",
            "ui_game_symbol_map_waypoint02",
            "ui_game_symbol_map_waypoint03",
            "ui_game_symbol_map_waypoint04",
            "ui_game_symbol_map_waypoint05",
            "ui_game_symbol_map_waypoint06",
            "ui_game_symbol_map_waypoint07",
            "ui_game_symbol_map_waypoint08",
            "ui_game_symbol_map_waypoint09",
            "ui_game_symbol_map_waypoint10",
            "ui_game_symbol_map_waypoint11",
            "ui_game_symbol_map_waypoint12",
            "ui_game_symbol_map_waypoint13",
            "ui_game_symbol_map_waypoint14",
            "ui_game_symbol_map_waypoint15",
            "ui_game_symbol_map_waypoint16",
            "ui_game_symbol_map_waypoint17",
            "ui_game_symbol_map_waypoint18",
            "ui_game_symbol_map_waypoint19",
            "ui_game_symbol_map_waypoint20",
            "ui_game_symbol_map_waypoint21",
            "ui_game_symbol_map_waypoint22",
            "ui_game_symbol_map_waypoint23",
            "ui_game_symbol_map_waypoint24",
            "ui_game_symbol_map_waypoint25",
            "ui_game_symbol_map_waypoint26",
            "ui_game_symbol_map_waypoint27",
            "ui_game_symbol_map_waypoint28",
            "ui_game_symbol_map_waypoint29",
            "ui_game_symbol_map_waypoint30",
            "ui_game_symbol_map_waypoint31",
            "ui_game_symbol_map_waypoint32",
            "ui_game_symbol_map_waypoint33",
            "ui_game_symbol_map_waypoint34",
            "ui_game_symbol_map_waypoint35",
            "ui_game_symbol_map_waypoint36",
            "ui_game_symbol_map_waypoint37",
            "ui_game_symbol_map_waypoint38",
            "ui_game_symbol_map_waypoint39",
            "ui_game_symbol_map_waypoint40",
            "ui_game_symbol_map_waypoint41",
            "ui_game_symbol_map_waypoint42",
            "ui_game_symbol_map_waypoint43",
            "ui_game_symbol_map_waypoint44",
            "ui_game_symbol_map_waypoint45",
            "ui_game_symbol_map_waypoint46",
            "ui_game_symbol_map_waypoint47",
            "ui_game_symbol_map_waypoint48"
        };

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < this.children.Count; i++)
            {
                XUiController xuiController = this.children[i].Children[0];
                if (xuiController is XUiC_MapSubPopupEntry)
                {
                    XUiC_MapSubPopupEntry xuiC_MapSubPopupEntry = (XUiC_MapSubPopupEntry)xuiController;
                    xuiC_MapSubPopupEntry.SetIndex(i);
                    xuiC_MapSubPopupEntry.SetSpriteName(XUiC_MapSubPopupList.sprites[i % XUiC_MapSubPopupList.sprites.Length]);
                }
            }
        }

        internal void ResetList()
        {
            for (int i = 0; i < this.children.Count; i++)
            {
                XUiController xuiController = this.children[i].Children[0];
                if (xuiController is XUiC_MapSubPopupEntry)
                {
                    ((XUiC_MapSubPopupEntry)xuiController).Reset();
                }
            }
        }
    }
}
