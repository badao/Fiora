using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Fiora
{
    static class Program
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, R;

        private static Menu Menu;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Fiora")
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W,750);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
            W.SetSkillshot(0.75f,80, 2000, false, SkillshotType.SkillshotLine);
            W.MinHitChance = HitChance.High;


            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Fiora.Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));

            Menu Harass = spellMenu.AddSubMenu(new Menu("Harass", "Harass"));

            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));

            Menu Focus = spellMenu.AddSubMenu(new Menu("Focus Selected", "Focus Selected"));

            Menu Clear = spellMenu.AddSubMenu(new Menu("Clear", "Clear"));

            Menu Draw = Menu.AddSubMenu(new Menu("Draw", "Draw")); ;


            spellMenu.AddItem(new MenuItem("Auto W", "Auto W targeted").SetValue(true));

            Harass.AddItem(new MenuItem("Use Q Harass", "Use Q Harass").SetValue(true));
            Harass.AddItem(new MenuItem("Use W Harass", "Use W Harass").SetValue(true));
            Harass.AddItem(new MenuItem("Use E Harass", "Use E Harass").SetValue(true));
            Harass.AddItem(new MenuItem("Mana Harass", "Mana Harass").SetValue(new Slider(40, 0, 100)));

            Combo.AddItem(new MenuItem("Use Q Combo", "Use Q Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use W Combo", "Use W Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use E Combo", "Use E Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use R Combo", "Use R Combo Selected").SetValue(true));

            Clear.AddItem(new MenuItem("Use E Clear", "Use E Clear").SetValue(true));
            Clear.AddItem(new MenuItem("Use Timat Clear", "Use Tiamat Clear").SetValue(true));
            Clear.AddItem(new MenuItem("minimum Mana LC", "minimum Mana Clear").SetValue(new Slider(40, 0, 100)));

            Draw.AddItem(new MenuItem("Draw Q", "Draw Q").SetValue(true));
            Draw.AddItem(new MenuItem("Draw W", "Draw W").SetValue(true));


            Menu.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.OnAttack += OnAttack;
            Obj_AI_Base.OnProcessSpellCast += oncast;
            Game.PrintChat("Welcome to FioraWorld");
        }
        private static bool Qharass { get { return Menu.Item("Use Q Harass").GetValue<bool>(); } }
        private static bool Wharass { get { return Menu.Item("Use W Harass").GetValue<bool>(); } }
        private static bool Eharass { get { return Menu.Item("Use E Harass").GetValue<bool>(); } }
        private static int Manaharass { get { return Menu.Item("Mana Harass").GetValue<Slider>().Value; } }
        private static bool Qcombo { get { return Menu.Item("Use Q Combo").GetValue<bool>(); } }
        private static bool Wcombo { get { return Menu.Item("Use W Combo").GetValue<bool>(); } }
        private static bool Ecombo { get { return Menu.Item("Use E Combo").GetValue<bool>(); } }
        private static bool Rcombo { get { return Menu.Item("Use R Combo").GetValue<bool>(); } }
        private static bool Eclear { get { return Menu.Item("Use E Clear").GetValue<bool>(); } }
        private static bool TimatClear { get { return Menu.Item("Use Timat Clear").GetValue<bool>(); } }
        private static int Manaclear { get { return Menu.Item("minimum Mana LC").GetValue<Slider>().Value; } }
        private static bool DrawQ { get { return Menu.Item("Draw Q").GetValue<bool>(); } }
        private static bool DrawW { get { return Menu.Item("Draw W").GetValue<bool>(); } }
        private static bool AutoW { get { return Menu.Item("Auto W").GetValue<bool>(); } }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            //foreach (var x in HeroManager.Enemies.Where(x => x.IsValidTarget()))
            //{
            //    if (HasPassive(x))
            //    {
            //        var pos1 = passivepos(x);
            //        var poses2 = PassiveRadiusPoint(x);
            //        var pos = passivepos(x) - x.Distance(Prediction.GetPrediction(x, Player.Distance(x.Position) / 1600).UnitPosition)
            //        * (x.Position - Prediction.GetPrediction(x, Player.Distance(x.Position) / 1600).UnitPosition).Normalized();
            //        if (pos.IsValid())
            //        {
            //            Render.Circle.DrawCircle(pos, 100, Color.Yellow);
            //        }
            //        if (pos1.IsValid())
            //        {
            //            Render.Circle.DrawCircle(pos1, 100, Color.Red);
            //        }
            //        foreach (var y in poses2.Where(y => y.IsValid()))
            //        {
            //            Render.Circle.DrawCircle(y, 100, Color.Violet);
            //        }

            //    }
            //}
            //foreach (var x in HeroManager.Enemies.Where(x => x.IsValidTarget()))
            //{
            //    if (HasUltiPassive(x))
            //    {
            //        var poses = UltiPassivePos(x);
            //        foreach (var y in poses)
            //        {
            //            Render.Circle.DrawCircle(y, 100, Color.Violet);
            //        }
            //    }
            //}
            if (DrawQ)
                Render.Circle.DrawCircle(Player.Position, 400, Color.Green);
            if (DrawW)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Green);
            }
        }
        public static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (AutoW && sender.IsEnemy && sender.IsChampion() && W.IsReady() && args.Target.IsMe && !args.SData.IsAutoAttack() &&
                (args.SData.TargettingType == SpellDataTargetType.SelfAndUnit || args.SData.TargettingType == SpellDataTargetType.Unit))
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    var x = W.GetPrediction(target).CastPosition;
                    W.Cast(x);
                }
            }
            if (!sender.IsMe)
                return;
            //Game.PrintChat(spell.Name);
            if (spell.Name.Contains("ItemTiamatCleave"))
            {

            }
            if (spell.Name.Contains("FioraQ"))
            {

            }
            if (spell.Name.Contains("FioraE"))
            {

                Orbwalking.ResetAutoAttackTimer();
            }
            if (spell.Name.ToLower().Contains("fiorabasicattack"))
            {
            }
        }
        public static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {

            if (unit.IsMe && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }

        }
        public static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Ecombo && E.IsReady())
                {
                    E.Cast();
                }
                else if (HasItem())
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && (unit is Obj_AI_Hero))
            {
                if (Eharass && E.IsReady())
                {
                    E.Cast();
                }
                else if (HasItem())
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (Eclear && E.IsReady() && Player.Mana*100/Player.MaxMana >= Manaclear)
                {
                    E.Cast();
                }
                else if (TimatClear && HasItem())
                {
                    CastItem();
                }
            }

        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            //checkobject2();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Qcombo)
                {
                    castQ();
                }
                if (Wcombo)
                {
                    castW();
                }
                if (Rcombo)
                {
                    castR();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (Qharass)
                {
                    castQ();
                }
                if (Wharass)
                {
                    castW();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
            }
        }
        private static void castR ()
        {
             var target = TargetSelector.GetSelectedTarget();
            if (target.IsValidTarget(500) && !target.IsZombie && R.IsReady())
            {
                R.Cast(target);
            }
        }
        private static void castW()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget() && !target.IsZombie && W.IsReady())
            {
                W.Cast(target);
            }
        }
        private static void castQ()
        {
            var target = TargetSelector.GetTarget(400, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget() && !target.IsZombie)
            {
                castQhelper(target);
            }
            else
            {
                target = TargetSelector.GetTarget(400 + Orbwalking.GetRealAutoAttackRange(Player), TargetSelector.DamageType.Physical);
                {
                    if (target.IsValidTarget() && !target.IsZombie)
                    {
                        castQhelper(target);
                    }
                    else
                    {
                        target = TargetSelector.GetTarget(400 + 350, TargetSelector.DamageType.Physical);
                        if (target.IsValidTarget() && !target.IsZombie)
                        {
                            castQhelper(target);
                        }
                    }
                }
            }
        }
        public static void castQhelper(Obj_AI_Base target)
        {
            if (HasPassive(target))
            {
                var poses = PassiveRadiusPoint(target);
                var pos = passivepos(target) - target.Distance(Prediction.GetPrediction(target, Player.Distance(target.Position) / 1600).UnitPosition)
                    * (target.Position - Prediction.GetPrediction(target, Player.Distance(target.Position) / 1600).UnitPosition).Normalized();
                var possibleposes = new List<Vector3>();
                for (int i = 0; i <= 400; i = i + 20)
                {
                    var p = Player.Position.Extend(pos, i);
                    possibleposes.Add(p);
                }
                var castpos = possibleposes.Where(x => x.InTheCone(poses, target.Position) && x.Distance(Prediction.GetPrediction(target, Player.Position.Distance(x) / 1600).UnitPosition) <= 300)
                                            .OrderByDescending(x => 1 - x.Distance(Prediction.GetPrediction(target, Player.Position.Distance(x) / 1600).UnitPosition))
                                            .FirstOrDefault();
                if (castpos != null)
                {
                    Q.Cast(castpos);
                }
                else 
                {
                    var pos1 = Player.Position.Extend(target.Position, 400);
                    if (Player.Distance(target.Position) < 400)
                        pos1 = target.Position;
                    if (pos1.Distance(target.Position) <= 300)
                    {
                        Q.Cast(pos1);
                    }
                }
            }
            else if (HasUltiPassive(target))
            {
                var poses = UltiPassivePos(target);
                var castpos = poses.OrderByDescending(x => 1 - x.Distance(Prediction.GetPrediction(target, 0.1f).UnitPosition)).FirstOrDefault();
                if (castpos != null)
                {
                    Q.Cast(castpos);
                }
                else
                {
                    var pos1 = Player.Position.Extend(target.Position, 400);
                    if (Player.Distance(target.Position) < 400)
                        pos1 = target.Position;
                    if (pos1.Distance(target.Position) <= 300)
                    {
                        Q.Cast(pos1);
                    }
                }
            }
            else
            {
                var pos1 = Player.Position.Extend(target.Position, 400);
                if (Player.Distance(target.Position) < 400)
                    pos1 = target.Position;
                if (pos1.Distance(target.Position) <= 300)
                {
                    Q.Cast(pos1);
                }
            }
        }
        public static bool HasItem()
        {
            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CastItem()
        {

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
        }
        public static bool HasPassive (Obj_AI_Base target)
        {
            return FioraPassiveObjects.Any(x => x.Position.Distance(target.Position) <= 50);
        }

        public static Vector3 passivepos (Obj_AI_Base target)
        {
            if (HasPassive(target))
            {
                var passive = FioraPassiveObjects.Where(x => x.Position.Distance(target.Position) <= 50).FirstOrDefault();
                if (passive != null)
                {
                    if (passive.Name.Contains("NE"))
                    {
                        var pos = new Vector2();
                        pos.X = target.Position.To2D().X;
                        pos.Y = target.Position.To2D().Y +150;
                        return pos.To3D();
                    }
                    if (passive.Name.Contains("SE"))
                    {
                        var pos = new Vector2();
                        pos.X = target.Position.To2D().X -150;
                        pos.Y = target.Position.To2D().Y;
                        return pos.To3D();
                    }
                    if (passive.Name.Contains("NW"))
                    {
                        var pos = new Vector2();
                        pos.X = target.Position.To2D().X + 150;
                        pos.Y = target.Position.To2D().Y;
                        return pos.To3D();
                    }
                    if (passive.Name.Contains("SW"))
                    {
                        var pos = new Vector2();
                        pos.X = target.Position.To2D().X;
                        pos.Y = target.Position.To2D().Y - 150;
                        return pos.To3D();
                    }
                    return new Vector3();
                }
                return new Vector3();
            }
            return new Vector3();
        }
        public static List<Vector3> PassiveRadiusPoint (Obj_AI_Base target)
        {
            if(HasPassive(target))
            {
                var passive = FioraPassiveObjects.Where(x => x.Position.Distance(target.Position) <= 50).FirstOrDefault();
                if (passive != null)
                {
                    if (passive.Name.Contains("NE"))
                    {
                        var pos1 = new Vector2();
                        var pos2 = new Vector2();
                        pos1.X = target.Position.To2D().X + 150 /(float) Math.Sqrt(2);
                        pos2.X = target.Position.To2D().X - 150 / (float)Math.Sqrt(2);
                        pos1.Y = target.Position.To2D().Y + 150 / (float)Math.Sqrt(2);
                        pos2.Y = target.Position.To2D().Y + 150 / (float)Math.Sqrt(2);
                        return new List<Vector3>() { pos1.To3D(), pos2.To3D() };
                    }
                    if (passive.Name.Contains("SE"))
                    {
                        var pos1 = new Vector2();
                        var pos2 = new Vector2();
                        pos1.X = target.Position.To2D().X - 150 / (float)Math.Sqrt(2);
                        pos2.X = target.Position.To2D().X - 150 / (float)Math.Sqrt(2);
                        pos1.Y = target.Position.To2D().Y - 150 / (float)Math.Sqrt(2);
                        pos2.Y = target.Position.To2D().Y + 150 / (float)Math.Sqrt(2);
                        return new List<Vector3>() { pos1.To3D(), pos2.To3D() };
                    }
                    if (passive.Name.Contains("NW"))
                    {
                        var pos1 = new Vector2();
                        var pos2 = new Vector2();
                        pos1.X = target.Position.To2D().X + 150 / (float)Math.Sqrt(2);
                        pos2.X = target.Position.To2D().X + 150 / (float)Math.Sqrt(2);
                        pos1.Y = target.Position.To2D().Y - 150 / (float)Math.Sqrt(2);
                        pos2.Y = target.Position.To2D().Y + 150 / (float)Math.Sqrt(2);
                        return new List<Vector3>() { pos1.To3D(), pos2.To3D() };
                    }
                    if (passive.Name.Contains("SW"))
                    {
                        var pos1 = new Vector2();
                        var pos2 = new Vector2();
                        pos1.X = target.Position.To2D().X + 150 / (float)Math.Sqrt(2);
                        pos2.X = target.Position.To2D().X - 150 / (float)Math.Sqrt(2);
                        pos1.Y = target.Position.To2D().Y - 150 / (float)Math.Sqrt(2);
                        pos2.Y = target.Position.To2D().Y - 150 / (float)Math.Sqrt(2);
                        return new List<Vector3>() { pos1.To3D(), pos2.To3D() };
                    }
                    return new List<Vector3>();
                }
                else
                {
                    return new List<Vector3>();
                }
            }
            else
            {
                return new List<Vector3>();
            }
        }
        public static bool InTheCone (this Vector3 pos, List<Vector3> poses, Vector3 targetpos)
        {
            bool x = true;
            foreach (var i in poses)
            {
                if (AngleBetween(pos.To2D(),targetpos.To2D(),i.To2D()) >90)
                    x = false;
            }
            return x;
        }
        private static bool HasUltiPassive (Obj_AI_Base target)
        {
            return ObjectManager.Get<GameObject>()
                .Where(x => x.Name.Contains("Fiora_Base_R_Mark") || (x.Name.Contains("Fiora_Base_R")&& x.Name.Contains("Timeout_FioraOnly.troy"))).Any(x => x.Position.Distance(target.Position) <= 50);
        }
        private static List<Vector3> UltiPassivePos (Obj_AI_Base target)
        {
            List<Vector3> poses = new List<Vector3>();
            if (HasUltiPassive(target))
            {
                var passive = ObjectManager.Get<GameObject>()
                    .Where(x => x.Name.Contains("Fiora_Base_R_Mark") || (x.Name.Contains("Fiora_Base_R") && x.Name.Contains("Timeout_FioraOnly.troy")));
                foreach (var x in passive)
                {
                    if (x.Name.Contains("NE"))
                    {
                        var pos = new Vector2();
                        pos.X = target.Position.To2D().X;
                        pos.Y = target.Position.To2D().Y + 150;
                        poses.Add(pos.To3D());
                    }
                    else if (x.Name.Contains("SE"))
                    {
                        var pos = new Vector2();
                        pos.X = target.Position.To2D().X - 150;
                        pos.Y = target.Position.To2D().Y;
                        poses.Add(pos.To3D());
                    }
                    else if (x.Name.Contains("NW"))
                    {
                        var pos = new Vector2();
                        pos.X = target.Position.To2D().X + 150;
                        pos.Y = target.Position.To2D().Y;
                        poses.Add(pos.To3D());
                    }
                    else if (x.Name.Contains("SW"))
                    {
                        var pos = new Vector2();
                        pos.X = target.Position.To2D().X;
                        pos.Y = target.Position.To2D().Y - 150;
                        poses.Add(pos.To3D());
                    }
                }
            }
            return poses;
        }
        private static void checkobject()
        {
            var target = ObjectManager.Get<GameObject>().Where(x => x.Position.Distance(Game.CursorPos) <= 200 && x.Name != "ardailker")
                .OrderByDescending(x => 1 - x.Position.Distance(Game.CursorPos)).FirstOrDefault(); ;
            String temp = "";
           temp += (" " + target.Name + " ");
            Game.PrintChat(temp);
        }
        private static void checkobject2()
        {
            var target = ObjectManager.Get<GameObject>().Where(x => x.Name.ToLower().Contains("fiora")
                && x.Name.ToLower().Contains("sw"));
            String temp = "";
            foreach(var x in target)
            {
                temp += (" " + x.Name + " ");
            }
            Game.PrintChat(temp);
        }
        private static List<GameObject> FioraPassiveObjects
        {
            get
            {
                var x = ObjectManager.Get<GameObject>().Where(a => FioraPassiveName.Contains(a.Name)).ToList();
                return x;
            }
        }
        private static List<string> FioraPassiveName = new List<string>()
        {
            "Fiora_Base_Passive_NE.troy",
            "Fiora_Base_Passive_SE.troy",
            "Fiora_Base_Passive_NW.troy",
            "Fiora_Base_Passive_SW.troy",
            "Fiora_Base_Passive_NE_Timeout.troy",
            "Fiora_Base_Passive_SE_Timeout.troy",
            "Fiora_Base_Passive_NW_Timeout.troy",
            "Fiora_Base_Passive_SW_Timeout.troy",
        };
        public static double AngleBetween(Vector2 a, Vector2 b, Vector2 c)
        {
            float a1 = c.Distance(b);
            float b1 = a.Distance(c);
            float c1 = b.Distance(a);
            if (a1 == 0 || c1 == 0) { return 0; }
            else
            {
                return Math.Acos((a1 * a1 + c1 * c1 - b1 * b1) / (2 * a1 * c1)) * (180 / Math.PI);
            }
        }
    }
}
