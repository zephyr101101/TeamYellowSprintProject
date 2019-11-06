using System.IO;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace GameLibrary {
    public class Map {
        private char[,] layout;
        private IDictionary<char, Tile> tileDict;
        public GroupBox grpMap { get; private set; }

        static private Character character;
        static public Map CurrentMap { get; private set; }
        static private GroupBox grpBox;
        static private IDictionary<string, Map> mapDict = new Dictionary<string, Map>();
        static private Func<string, Bitmap> loadImg;

        private const int TOP_PAD = 10;
        private const int BOUNDARY_PAD = 5;
        private const int BLOCK_SIZE = 50;
        static public double encounterChance;
        static private Random rand;

        public int CharacterStartRow { get; private set; }
        public int CharacterStartCol { get; private set; }
        private int NumRows { get { return layout.GetLength(0); } }
        private int NumCols { get { return layout.GetLength(1); } }

        /// <summary>
        /// Change the map
        /// </summary>
        /// <param name="mapFolder">String representing the map to be changed to</param>
        public static void ChangeMap(string mapFolder) {
            // if there is a CurrentMap, hide it
            if (CurrentMap != null) {
                CurrentMap.grpMap.Hide();
            }
            // if the desired map already exists in mapDict, show it and set it as the current map
            if (mapDict.ContainsKey(mapFolder)) {
                mapDict[mapFolder].grpMap.Show();
                CurrentMap = mapDict[mapFolder];
            }
            // otherwise, make it (it will automatically be shown and set itself as current)
            else {
                new Map().LoadMap(mapFolder);
            }

            // update window size to accommodate new map
            grpBox.Height = CurrentMap.grpMap.Height;
            grpBox.Width = CurrentMap.grpMap.Width;
        }

        
        public static Character InitializeMaps(string firstMapFolder, string characterImage, GroupBox grpBox, Func<string, Bitmap> LoadImg) {
            Map.grpBox = grpBox;
            loadImg = LoadImg;

            Map map = new Map();
            map.LoadMap(firstMapFolder);

            // resize GroupBox to match loaded map
            grpBox.Width = CurrentMap.grpMap.Width;
            grpBox.Height = CurrentMap.grpMap.Height;
            grpBox.Top = 5;
            grpBox.Left = 5;

            // initialize for game
            encounterChance = 0.15;
            rand = new Random();
            Game.GetGame().ChangeState(GameState.ON_MAP);

            // initialize character picture box
            PictureBox pb = new PictureBox() {
                Image = LoadImg(characterImage),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = BLOCK_SIZE,
                Height = BLOCK_SIZE,
                BackgroundImageLayout = ImageLayout.Stretch
            };

            Position p = new Position(map.CharacterStartRow, map.CharacterStartCol);
            Position pPB = RowColToTopLeft(p);

            pb.Top = pPB.row;
            pb.Left = pPB.col;
            grpBox.Controls.Add(pb);
            pb.BringToFront();

            // initialize the character and return it
            character = new Character(pb, p);
            return character;
        }

        public void LoadMap(string mapFolder) {
            mapDict.Add(mapFolder, this);
            grpMap = new GroupBox();
            grpBox.Controls.Add(grpMap);
            CurrentMap = this;

            mapFolder = @"Resources\" + mapFolder;

            // declare and initialize locals
            int top = TOP_PAD;
            int left = BOUNDARY_PAD;
            char startCharacter = ' ';
            List<string> mapLines = new List<string>();
            List<string> mapTiles = new List<string>();

            // read from tiles file
            using (FileStream fs = new FileStream(mapFolder + @"\Tiles.txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        mapTiles.Add(line);
                        line = sr.ReadLine();
                    }
                }
            }

            tileDict = new Dictionary<char, Tile>();

            // read character-tile relationships from file
            foreach (string mapTile in mapTiles)
            {
                // break line up
                string[] words = mapTile.Split(' ');
                char c = words[0][0]; // first character is the character described by the tile
                Tile t;

                // construct the tile by the tile type (the second word)
                switch (words[1])
                {
                    case "start": // start is a keyword defining the start character but is really just a path tile
                        startCharacter = c;
                        t = new PathTile(words[2]);
                        break;
                    case "path":
                        t = new PathTile(words[2]);
                        break;
                    case "wall":
                        t = new WallTile(words[2]);
                        break;
                    case "inportal":
                        t = new InportalTile(words[2][0], words[3]);
                        break;
                    case "outportal":
                        t = new OutportalTile(words[2][0], words[3], words[4]);
                        break;
                    default:
                        t = new WallTile("blank");
                        break;
                }

                // add the char, tile pair to tileDict
                tileDict.Add(c, t);
            }

            // read from map file
            using (FileStream fs = new FileStream(mapFolder + @"\Map.txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        mapLines.Add(line);
                        line = sr.ReadLine();
                    }
                }
            }

            // load map file into layout and create PictureBox objects
            layout = new char[mapLines.Count, mapLines[0].Length];
            int i = 0;
            foreach (string mapLine in mapLines) {
                int j = 0;
                foreach (char c in mapLine) {
                    layout[i, j] = c;
                    PictureBox pb = tileDict[c].MakePictureBox(loadImg);
                    if (pb != null) {
                        pb.Top = top;
                        pb.Left = left;
                        grpMap.Controls.Add(pb);
                    }
                    if (c == startCharacter) {
                        CharacterStartRow = i;
                        CharacterStartCol = j;
                    }
                    left += BLOCK_SIZE;
                    j++;
                }
                left = BOUNDARY_PAD;
                top += BLOCK_SIZE;
                i++;
            }

            // resize Group
            grpMap.Width = NumCols * BLOCK_SIZE + BOUNDARY_PAD * 2;
            grpMap.Height = NumRows * BLOCK_SIZE + TOP_PAD + BOUNDARY_PAD;
        }

        public Position? Enter(Position pos)
        {
            // if the position is outside of the map, return null
            if (pos.row < 0 || pos.row >= NumRows ||
                pos.col < 0 || pos.col >= NumCols) {
                return null;
            }

            // based on the tile for the given position's char, enter it
            Position? newPos = tileDict[layout[pos.row, pos.col]].Enter(pos);

            // if it returned null, the character does not move so return null
            if (newPos == null) {
                return null;
            }

            // otherwise see if the character should get a random encounter
            if (rand.NextDouble() < encounterChance) {
                encounterChance = 0.15;
                Game.GetGame().ChangeState(GameState.FIGHTING);
            }
            else {
                encounterChance -= 0.10;
            }

            // return the new position
            return newPos;
        }

        /// <summary>
        /// Converts a row, column position into a position based on pixel location
        /// </summary>
        /// <param name="p">The row, col position to be converted pixelwise</param>
        /// <returns></returns>
        public static Position RowColToTopLeft(Position p) {
            return new Position(p.row * BLOCK_SIZE + TOP_PAD, p.col * BLOCK_SIZE + BOUNDARY_PAD);
        }

        /// <summary>
        /// Finds the position of a character in a level layout
        /// </summary>
        /// <param name="c">The character to be found</param>
        /// <returns></returns>
        public Position? PositionOfCharacter(char c) {
            // Go through each character in the layout until it is found
            for (int i = 0; i < NumRows; i++) {
                for (int j = 0; j < NumCols; j++) {
                    if (layout[i, j] == c) return new Position(i, j); // if it is found return its position
                }
            }

            // if it is not found return null
            return null;
        }

        public Bitmap GetBackgroundImage(Position p) {
            string imageFile = tileDict[layout[p.row, p.col]].ImageFile;
            return imageFile == "blank" ? null : loadImg(imageFile);
        }

        /// <summary>
        /// Basic tile class for tiles
        /// </summary>
        private abstract class Tile {
            public string ImageFile { get; private set;  }
            
            public Tile(string imageFile) {
                ImageFile = imageFile;
            }

            public abstract Position? Enter(Position pos); // all tiles must implement a function for when the character steps into the tile

            /// <summary>
            /// Make a picture box based on this tile type
            /// </summary>
            /// <param name="LoadImg"></param>
            /// <returns></returns>
            public PictureBox MakePictureBox(Func<string, Bitmap> LoadImg) {
                if (ImageFile == "blank") return null; // if the imagefile is the keyword blank, return null
                // otherwise make a picture box
                else return new PictureBox() {
                    BackgroundImage = LoadImg(ImageFile),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    Width = BLOCK_SIZE,
                    Height = BLOCK_SIZE
                };
            }
        }

        private class PathTile : Tile {
            public PathTile(string imageFile) : base(imageFile) { }

            public override Position? Enter(Position pos) {
                // let the character step into the tile
                return pos;
            }
        }

        private class WallTile : Tile {
            public WallTile(string imageFile) : base(imageFile) { }
            public override Position? Enter(Position pos) {
                // do not let the character step into the tile
                return null;
            }
        }

        private class InportalTile : Tile
        {
            private char destination;

            public InportalTile(char destination, string imageFile) : base(imageFile) {
                this.destination = destination;
            }

            public override Position? Enter(Position pos)
            {
                // find the position of the destination of the portal and send the character there
                return CurrentMap.PositionOfCharacter(destination);
            }
        }

        private class OutportalTile : Tile
        {
            private char destination;
            private string level;

            public OutportalTile(char destination, string level, string imageFile) : base(imageFile) {
                this.destination = destination;
                this.level = level;
            }

            public override Position? Enter(Position pos) {
                // switch to the right level then send the character to the position of the destination of the portal
                ChangeMap(level);
                return CurrentMap.PositionOfCharacter(destination);
            }
        }
    }
}
