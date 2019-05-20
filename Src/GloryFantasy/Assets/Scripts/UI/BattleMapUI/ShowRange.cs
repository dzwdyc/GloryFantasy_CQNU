using UnityEngine;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;

using GameUnit;

namespace GameGUI
{

    public class ShowRange : MonoBehaviour
    {
        private int columns;
        private int rows;
        private Unit unit;
        private bool unitMove;
        private void Awake()
        {
            this.unit = gameObject.GetComponent<Unit>();
        }

        private void Start()
        {

            columns = BattleMap.BattleMap.Instance().Columns;
            rows = BattleMap.BattleMap.Instance().Rows;
        }

        
        private List<Vector2> GetPositionsWithinCertainMd(Vector2 position, int ManhattanDistance)
        {
            List<Vector2> reslist = new List<Vector2>();
            if (unitMove)
            {
                RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);
                RemoveMapBlokHasUnit(reslist);
            }
            else
            {
                RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);
            }
            return reslist;

        }

        private void RecrusiveBody(int x, int y, int leftManhattanDistance, List<Vector2> reslist)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            reslist.Add(new Vector2(x, y));
            if (leftManhattanDistance == 0)
                return;
            RecrusiveBody(x + 1, y, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x - 1, y, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x, y + 1, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x, y - 1, leftManhattanDistance - 1, reslist);
        }

        private void RecrusiveBody2(int x, int y, int range, List<Vector2> reslist)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            Vector2 centPosition = new Vector2(x, y);
            int tempRange = range%2 == 0 ? range / 2 - 1 : (range - 1) / 2 ;
            int starPosition_x =(int)centPosition.x - tempRange;
            int starPosition_y = (int)centPosition.y - tempRange;
            for(int i = 0;i < range; i++)
            {
                for(int j = 0; j < range; j++)
                {
                    reslist.Add(new Vector2(starPosition_x + j, starPosition_y + i));
                }
            }
        }


        //�ƶ���Χ����ʾ�е�λ�ĵ�ͼ��
        //TODO����ʾ�޷�����ĵ�ͼ��
        private void RemoveMapBlokHasUnit(List<Vector2> reslist)
        {
            for (int i = 0; i < reslist.Count; i++)
            {
                for (int j = reslist.Count - 1; j > i; j--)
                {

                    if (reslist[i] == reslist[j])
                    {
                        reslist.RemoveAt(j);
                    }
                }
            }
            for (int i = 0; i < reslist.Count; i++)
            {
                if (BattleMap.BattleMap.Instance().CheckIfHasUnits(reslist[i]))
                {
                    reslist.Remove(reslist[i]);
                }
            }
        }

        private void RecrusiveBodyForSkill(int x,int y,int range,List<Vector2> reslist)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            reslist.Add(new Vector2(x, y));
            if (range == 0) return;
            if(range == 2 || range == 4)
            {
                if(range == 2)
                {
                    range = range - 1;
                }
                else if(range == 4)
                {
                    range = range - 2;
                }
                RecrusiveBody(x, y, range, reslist);
            }
            if(range == 3 || range == 6)
            {
                if (range == 6) range = range - 1;
                RecrusiveBody2(x, y, range, reslist);
            }
            if(range == 5)
            {
                //TODO
                RecrusiveBody2(x, y, range, reslist);
            }
        }

        /// <summary>
        /// ���ؼ��ܷ�Χ�ڵ����е�ͼ���������б�
        /// </summary>
        /// <param name="position">��������</param>
        /// <param name="range">��Χ��1-6��</param>
        /// <returns></returns>
        public List<Vector2> GetSkillRnage(Vector2 position, int range)
        {
            List<Vector2> reslist = new List<Vector2>();
            RecrusiveBodyForSkill((int)position.x, (int)position.y,range, reslist);
            return reslist;
        }

        /// <summary>
        /// ������λ�ƶ���Χ
        /// </summary>
        /// <param name="target">��λ����</param>
        public void MarkMoveRange(Vector2 target)
        {
            unitMove = true;
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, unit.mov), Color.green);
        }

        /// <summary>
        /// ������λ������Χ
        /// </summary>
        /// <param name="target">��λ����</param>
        public void MarkAttackRange(Vector2 target)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, unit.rng), Color.red);
        }

        /// <summary>
        /// ȡ����λ�ƶ���Χ����
        /// </summary>
        /// <param name="target"></param>
        public void CancleMoveRangeMark(Vector2 target)
        {
            unitMove = false;
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                 GetPositionsWithinCertainMd(target, unit.mov), Color.white);   
        }

        /// <summary>
        /// ȡ����λ������Χ����
        /// </summary>
        /// <param name="target"></param>
        public void CancleAttackRangeMark(Vector2 target)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                 GetPositionsWithinCertainMd(target, unit.rng), Color.white);
        }

        /// <summary>
        /// �������ܷ�Χ
        /// </summary>
        /// <param name="target">��λ����</param>
        /// <param name="range">���ܷ�Χ����Χ�ȼ���1-6����</param>
        public void MarkSkillRange(Vector2 target, int range)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetSkillRnage(target, range), Color.red);
        }

        /// <summary>
        /// ȡ�����ܷ�Χ����
        /// </summary>
        /// <param name="target">��λ����</param>
        /// <param name="range">���ܷ�Χ����Χ�ȼ���1-6����</param>
        public void CancleSkillRangeMark(Vector2 target,int range)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, range), Color.white);
        }
    }
}