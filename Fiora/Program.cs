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

namespace Fiora
{
    static class Program
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, R;

        private static Menu Menu;

        private static float l, k, lastAA , Qcount, Qstate;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Fiora")
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
            //Q.SetSkillshot(300, 50, 2000, false, SkillshotType.SkillshotLine);


            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Fiora.Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            spellMenu.AddItem(new MenuItem("Use Q Harass", "Use Q Harass").SetValue(true));
            //spellMenu.AddItem(new MenuItem("Use W Harass", "Use W Harass").SetValue(true));
            //spellMenu.AddItem(new MenuItem("Use E Harass", "Use E Harass").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use Q Combo", "Use Q Combo").SetValue(true));
            spellMenu.AddItem(new MenuItem("Q minium distance", "Q minium distance").SetValue(new Slider(0, 0, 300)));
            //spellMenu.AddItem(new MenuItem("Use W Combo", "Use W Combo").SetValue(true));
            //spellMenu.AddItem(new MenuItem("Use E Combo", "Use E Combo").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use R Combo Burst", "Use R Combo Burst").SetValue(false));
            spellMenu.AddItem(new MenuItem("Use R Combo Killable", "Use R Combo Killable").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use R Combo Save Life", "Use R Save Life").SetValue(true));
            spellMenu.AddItem(new MenuItem("If HP <", "If HP <").SetValue(new Slider(20, 0, 100)));
            spellMenu.AddItem(new MenuItem("dont W if mana <", "dont W if mana <").SetValue(new Slider(40, 0, 100)));
            spellMenu.AddItem(new MenuItem("force focus selected", "force focus selected").SetValue(false));
            spellMenu.AddItem(new MenuItem("if selected in :", "if selected in :").SetValue(new Slider(1000, 1000, 1500)));
            //spellMenu.AddItem(new MenuItem("Use E", "Use E")).SetValue(false);
            //foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
            //{
            //    spellMenu.AddItem(new MenuItem("use R" + hero.SkinName, "use R" + hero.SkinName)).SetValue(true);
            //}

            //spellMenu.AddItem(new MenuItem("useR", "Use R to Farm").SetValue(true));
            //spellMenu.AddItem(new MenuItem("LaughButton", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            //spellMenu.AddItem(new MenuItem("ConsumeHealth", "Consume below HP").SetValue(new Slider(40, 1, 100)));

            Menu.AddToMainMenu();

            //Drawing.OnDraw += Drawing_OnDraw;

            Game.OnGameUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.OnAttack += OnAttack;
            Obj_AI_Base.OnProcessSpellCast += oncast;
            Game.PrintChat("Welcome to FioraWorld");
        }
        public static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
                return;
            //Game.PrintChat(spell.Name);
            if (spell.Name.Contains("ItemTiamatCleave"))
            {
                k = 0;
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    l = 0;
                }
                Game.Say("/l");
            }
            if (spell.Name.Contains("FioraQ"))
            {
                Qcount = Environment.TickCount;
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    l = 1;
                }
            }
            if (spell.Name.Contains("FioraFlurry"))
            {
                //Utility.DelayAction.Add(30, () => Orbwalking.ResetAutoAttackTimer());
                Orbwalking.ResetAutoAttackTimer();
            }
            if (spell.Name.ToLower().Contains("fiorabasicattack"))
            {
                lastAA = Environment.TickCount;
            }
        }
        public static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }
            if (target.IsMe)
            {
                Game.PrintChat(unit.Name);
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
                {
                    if (unit.Name.ToLower().Contains(hero.SkinName.ToLower()) && Player.Mana / Player.MaxMana > Menu.Item("dont W if mana <").GetValue<Slider>().Value)
                    {
                        var x = Player.Position;
                        W.Cast(x);
                    }

                }
            }
        }
        public static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed )
            {
                if (!E.IsReady() && HasItem())
                {
                    k = 1;
                }
                if (!E.IsReady() && !HasItem())
                {
                    l = 0;
                }
                if (E.IsReady() && Player.Mana >= E.Instance.ManaCost)
                {
                    var x = Player.Position;
                    E.Cast(x);
                }
                else
                {
                    if (HasItem())
                    {
                        CastItem();
                    }
                }
            }
        }
        public static void getItem()
        {
            if (k == 1)
            {
                CastItem();
            }
        }
        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            //if (Player.IsWindingUp)
            //{
            //    Game.PrintChat("heis");
            //}
            //foreach (var buff in Player.Buffs)
            //{
            //    string x = "";
            //    x += (buff.Name + "(" + buff.Count + ")" + ", ");
            //    Game.PrintChat(x);
            //}
            //if (Player.HasBuff("talonnoxiandiplomacybuff"))
            //{
            //    Game.PrintChat("alright");
            //}
            //Game.PrintChat(Q.Instance.Name);
            getQ();
            getItem();
            GetQstate();
            //WanhDc();
            if (Selected() == true && !Orbwalker.InAutoAttackRange(TargetSelector.GetSelectedTarget()) && !Player.IsWindingUp)
            {
                Orbwalker.SetAttack(false);
            }
            else
            {
                Orbwalker.SetAttack(true);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Menu.Item("Use Q Combo").GetValue<bool>())
                {
                    useQ();
                }
                //if (Menu.Item("Use W Combo").GetValue<bool>())
                //{
                //    useW();
                //}
                //if (Menu.Item("Use E Combo").GetValue<bool>())
                //{
                //    useE();
                //}
                if (Menu.Item("Use R Combo Burst").GetValue<bool>())
                {
                    useR();
                }
                if (Menu.Item("Use R Combo Killable").GetValue<bool>())
                {
                    useRKS();
                }
                if (Menu.Item("Use R Combo Save Life").GetValue<bool>())
                {
                    useRSL();
                }


            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (Menu.Item("Use Q Harass").GetValue<bool>())
                {
                    useQ();
                }
                //if (Menu.Item("Use W Harass").GetValue<bool>())
                //{
                //    useW();
                //}
                //if (Menu.Item("Use E Harass").GetValue<bool>())
                //{
                //    useE();
                //}

            }
        }
        public static bool Selected()
        {
            if (!Menu.Item("force focus selected").GetValue<bool>())
            {
                return false;
            }
            else
            {
                var target = TargetSelector.GetSelectedTarget();
                float a = Menu.Item("if selected in :").GetValue<Slider>().Value;
                if (target == null || target.IsDead || target.IsZombie)
                {
                    return false;
                }
                else
                {
                    if (Player.Distance(target.Position) > a)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public static Obj_AI_Base gettarget(float range)
        {
            if (Selected())
            {
                return TargetSelector.GetSelectedTarget();
            }
            else
            {
                return TargetSelector.GetTarget(range, TargetSelector.DamageType.Physical);
            }
        }

        public static void useQ()
        {
            var target = gettarget(600);
            if (Player.Distance(target.Position) <= 600)
            {
                //if (target != null && target.IsValidTarget() && !target.IsZombie && Orbwalking.CanAttack() && Q.IsReady() && l == 0 && !Orbwalker.InAutoAttackRange(target))
                //{
                //    Q.Cast(target);
                //}
                //if (target != null && target.IsValidTarget() && !target.IsZombie && Q.IsReady() && l == 0 && Orbwalker.InAutoAttackRange(target))
                //{
                //    Q.Cast(target);
                //}
                //if (Selected() && !Orbwalker.InAutoAttackRange(target))
                //{
                //    Q.Cast(target);
                //}
                var x = Menu.Item("Q minium distance").GetValue<Slider>().Value;
                if (target != null && target.IsValidTarget() && !target.IsZombie && WanhDc() && Q.IsReady() && l == 0 && Player.Distance(target.Position) >= x)
                {
                    Q.Cast(target);
                }
                if (target != null && target.IsValidTarget() && !target.IsZombie && WanhDc() && Q.IsReady() && l == 0 && Qstate == 2)
                {
                    Q.Cast(target);
                }
                var Qdmg = Damage.GetSpellDamage(Player,target, SpellSlot.Q);
                var dmg = Damage.CalcDamage(Player, target, Damage.DamageType.Physical, Player.BaseAttackDamage + Player.FlatPhysicalDamageMod);
                bool kill = Qdmg + dmg >= target.Health;
                if (target != null && target.IsValidTarget() && !target.IsZombie && WanhDc() && Q.IsReady() && l == 0 && kill)
                {
                    Q.Cast(target);
                }
            }

        }
        public static void getQ()
        {
            var target = gettarget(270);
            if (target == null && !Player.IsDashing())
                l = 0;
            if (!Orbwalking.InAutoAttackRange(target) && !Player.IsDashing())
                l = 0;

        }
        public static void useR()
        {
            var target = gettarget(400);
            if (R.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie && !Q.IsReady() && !E.IsReady() && l == 0 && Player.Distance(target.Position) <= 400)
            {
                if (Orbwalker.InAutoAttackRange(target) && !WanhDc() && !Player.IsWindingUp)
                {
                    R.Cast(target);
                }
                if (!Orbwalker.InAutoAttackRange(target) && !Player.IsWindingUp)
                {
                    R.Cast(target);
                }
            }
        }
        public static void useRKS()
        {
            var target = gettarget(400);
            var damage = new double[] { 300, 475, 650 }[R.Level - 1] + 0.9 * Player.FlatPhysicalDamageMod;
            double truedmg;
            if (target.CountEnemiesInRange(700) == 1)
            {
                truedmg = damage * 2.6;
            }
            else if ( target.CountEnemiesInRange(700) == 2)
            {
                truedmg = damage * 1.8;
            }
            else
            {
                truedmg = damage * 1.4;
            }
            if (R.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie && !Q.IsReady() && !E.IsReady() && l == 0 && Player.Distance(target.Position) <= 400)
            {
                if (Damage.CalcDamage(Player,target,Damage.DamageType.Physical,truedmg) > target.Health)
                {
                    if (Orbwalker.InAutoAttackRange(target) && !WanhDc() && !Player.IsWindingUp)
                    {
                        R.Cast(target);
                    }
                    if (!Orbwalker.InAutoAttackRange(target) && !Player.IsWindingUp)
                    {
                        R.Cast(target);
                    }
                }
            }



        }
        public static void useRSL()
        {
            var target = gettarget(400);
            if ( Player.Health/Player.MaxHealth*100 <= Menu.Item("If HP <").GetValue<Slider>().Value)
            {
                if( target != null && target.IsValidTarget() && !target.IsZombie && Player.Distance(target.Position) <= 400 )
                {
                    R.Cast(target);
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
        public static bool WanhDc()
        {
            return Environment.TickCount + Game.Ping / 2 + 25 >= lastAA + Player.AttackDelay * 1000;
        }
        public static void GetQstate()
        {
            if (Environment.TickCount - Qcount >= 4000)
            {
                Qstate = 0;
            }
            if (Environment.TickCount - Qcount >=3500 && Environment.TickCount - Qcount < 4000 && Qstate == 1)
            {
                Qstate = 2;
            }
            if (Environment.TickCount - Qcount <= 4000 && Qstate ==0 )
            {
                Qstate = 1;
            }

        }

    }
}
