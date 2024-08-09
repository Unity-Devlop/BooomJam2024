using cfg;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public struct ArrayPos
    {
        public int x;
        public int y;

        public ArrayPos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class DailyTrainTable : MonoBehaviour
    {
        private int tableWidth = 4;
        private int tableHeight;
        private RectTransform rectTransform;
        private float gridWidth = 100;
        private float gridHeight = 100;
        private ArrayPos[,] grids;
        private Dictionary<ArrayPos, PlasticGrid> posToGrid = new Dictionary<ArrayPos, PlasticGrid>();

        private void Awake()
        {
            tableHeight = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas.Count;
            InitGrids();
            rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(tableWidth * gridWidth, tableHeight * gridHeight);
        }

        private void InitGrids()
        {
            if (tableWidth <= 0) tableWidth = 1;
            if (tableHeight <= 0) tableHeight = 1;
            grids = new ArrayPos[tableHeight, tableWidth];
            for (int i = 0; i < tableHeight; ++i)
            {
                for (int j = 0; j < tableWidth; ++j)
                {
                    grids[i, j].x = -1;
                    grids[i, j].y = -1;
                }
            }
        }

        public void RemoveGrid(ArrayPos arrayPos, bool isHiden)
        {
            int l1 = arrayPos.y + posToGrid[arrayPos].heigth;
            for (int i = arrayPos.y, k = 0; i < l1; ++i, ++k)
            {
                int l2 = arrayPos.x + posToGrid[arrayPos].realSize[k];
                for (int j = arrayPos.x; j < l2; ++j)
                {
                    grids[i, j].x = -1;
                    grids[i, j].y = -1;
                }
            }

            if (isHiden) posToGrid[arrayPos].ResetPos();
            posToGrid.Remove(arrayPos);
            //Debug.Log($"remove {arrayPos.x} {arrayPos.y}");
        }

        public void RemoveGrid(Vector2 pos, bool isHiden)
        {
            int posY = (int)Mathf.Ceil(pos.y / gridHeight);
            int posX = (int)Mathf.Floor(pos.x / gridWidth);
            posY = -Mathf.Min(posY, 0);
            posX = Mathf.Max(posX, 0);
            ArrayPos arrayPos = new ArrayPos(posX, posY);
            if (posToGrid.ContainsKey(arrayPos))
            {
                int l1 = arrayPos.y + posToGrid[arrayPos].heigth;
                for (int i = arrayPos.y, k = 0; i < l1; ++i, ++k)
                {
                    int l2 = arrayPos.x + posToGrid[arrayPos].realSize[k];
                    for (int j = arrayPos.x; j < l2; ++j)
                    {
                        grids[i, j].x = -1;
                        grids[i, j].y = -1;
                    }
                }

                if (isHiden) posToGrid[arrayPos].ResetPos();
                posToGrid.Remove(arrayPos);
                //Debug.Log($"remove {arrayPos.x} {arrayPos.y}");
            }
        }

        public bool AddTrainGrid(PlasticGrid plasticGrid)
        {
            Vector2 pos = plasticGrid._RectTransform.anchoredPosition;
            int posY = (int)Mathf.Ceil(pos.y / gridHeight);
            int posX = (int)Mathf.Floor(pos.x / gridWidth);
            posY = -Mathf.Min(posY, 0);
            posX = Mathf.Max(posX, 0);
            int X = posX + plasticGrid.width;
            int Y = posY + plasticGrid.heigth;
            if (X <= tableWidth && Y <= tableHeight)
            {
                for (int i = posY, k = 0; i < Y; ++i, ++k)
                {
                    X = posX + plasticGrid.realSize[k];
                    for (int j = posX; j < X; ++j)
                    {
                        if (grids[i, j].x != -1 && grids[i, j].y != -1 && posToGrid.ContainsKey(grids[i, j]))
                        {
                            RemoveGrid(grids[i, j], true);
                        }

                        grids[i, j].x = posX;
                        grids[i, j].y = posY;
                    }
                }

                posToGrid.Add(new ArrayPos(posX, posY), plasticGrid);
                plasticGrid._RectTransform.anchoredPosition = new Vector2(posX * gridWidth, -posY * gridHeight);
                //Debug.Log($"add {posX} {posY}");
                return true;
            }

            return false;
        }

        public void Train(List<HuluData> datas)
        {
            foreach (var item in posToGrid)
            {
                int h = item.Key.y + item.Value.heigth;
                for (int i = item.Key.y; i < h; ++i)
                {
                    item.Value.trainContent.Train(datas[i]);
                }
            }
        }
    }
}