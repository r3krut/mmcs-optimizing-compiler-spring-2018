﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode
{
    // Список мертвых переменных
    using DVars = List<DUVar>;
    // Список живых переменных
    using LVars = List<DUVar>;

    /// <summary>
    /// Класс для определения живых и мертвых переменных
    /// и удаления метвого кода
    /// </summary>
    public class LiveAndDeadVariables
    {
        /// <summary>
        /// Базовый блок (исходный)
        /// </summary>
		public BasicBlock Block { get; }
        /// <summary>
        /// Базовый блок без мертвого кода
        /// </summary>
		public BasicBlock BlockNew { get; }
        /// <summary>
        /// Список мертвых переменных
        /// </summary>
        public DVars DeadVars { get; }
        /// <summary>
        /// Список живых переменных
        /// </summary>
        public LVars LiveVars { get; }
        /// <summary>
        /// Use переменные, которые не определены в блоке
        /// </summary>
        public List<DUVar> UListNotValid { get; }

        private List<Guid> removeVars;

        /// <summary>
        /// Конструктор для класса определения
        /// живых и мертвых переменных для
        /// базового блока
        /// </summary>
        /// <param name="block">Базовый блок</param>
        public LiveAndDeadVariables(BasicBlock block)
        {
            this.Block = block;
            this.UListNotValid = (new DULists(block)).UListNotValid;
            DeadVars = new DVars();
            LiveVars = new LVars();
            removeVars = new List<Guid>();
            DefineLDVars(Block.CodeList.ToList(), DeadVars, LiveVars);
        }

        /// <summary>
        /// Конструктор для класса определения
        /// живых и мертвых переменных для произвольного
        /// фрагмента кода
        /// </summary>
        /// <param name="nodes">Фрагмент кода</param>
        public LiveAndDeadVariables(List<Node> nodes) : this(new BasicBlock(nodes)) { }

        /// <summary>
        /// Определение живых и мертвых переменных
        /// в базовом блоке
        /// </summary>
        /// <param name="nodes">Фрагмент кода</param>
        private void DefineLDVars(List<Node> nodes, DVars DeadVars, LVars LiveVars)
        {
            // Построение Def цепочки по блоку
            var dList = (new DULists(nodes)).DList;

            // Обход Def цепочки
            foreach (var dVar in dList)
            { 
                // Мертвая переменная (нигде не используется)
                if (dVar.UseVariables.Count == 0)
                    DeadVars.Add(dVar.DefVariable);
                // Живые переменные
                else
                {
                    LiveVars.Add(dVar.DefVariable);

                    foreach (var uVar in dVar.UseVariables)
                        LiveVars.Add(uVar);
                }
            }
        }

        /// <summary>
        /// Делает мертвыми не определенные use-переменные 
        /// </summary>
        private void DefineUnDefVariables(List<Node> nodes)
        {
            foreach (var unDefV in this.UListNotValid)
                DeadVars.Add(unDefV);
        }

        /// <summary>
        /// Удаление мертвого кода в блоке
        /// </summary>
        public BasicBlock RemoveDeadCode()
        {
            var listNode = Block.CodeList.ToList();
            DefineUnDefVariables(listNode);
            var deadVars = DeadVars;
            var liveVars = LiveVars;

            // Пока мы не удалим все мертвые переменные
            while (deadVars.Count != 0)
            {
                foreach (var dV in removeVars)
                {
                    var ind = listNode.FindIndex(x => x.Label == dV);
                    listNode.RemoveAt(ind);
                }

                deadVars.Clear();
                liveVars.Clear();
                removeVars.Clear();

                // Определение живости/мертвости переменных
                DefineLDVars(listNode, deadVars, liveVars);
                DefineUnDefVariables(listNode);

                foreach (var dV in deadVars)
                    removeVars.Add(dV.StringId);

                removeVars = removeVars.Distinct().ToList();
            }
            
            return new BasicBlock(listNode.ToList());
        }

        public override string ToString()
        {
            return Block.ToString();
        }

        public override bool Equals(object obj)
        {
            return Block.Equals((obj as LiveAndDeadVariables).Block) &&
                DeadVars.Equals((obj as LiveAndDeadVariables).DeadVars) &&
                LiveVars.Equals((obj as LiveAndDeadVariables).LiveVars);
        }

        public override int GetHashCode()
        {
            return Block.GetHashCode() + DeadVars.GetHashCode() +
                LiveVars.GetHashCode();
        }
    }
}
