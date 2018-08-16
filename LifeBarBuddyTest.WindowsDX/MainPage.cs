using FilenameBuddy;
using GameTimer;
using LifeBarBuddy;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeBarBuddyTest.WindowsDX
{
	public class MainPage : WidgetScreen
	{
		#region Properties

		const float maxHP = 100f;
		const float maxMana = 100f;
		const float maxTime = 6f;

		float hitPoints;
		float mana;

		ILifeBar lifeBar;
		ISuperBar manaBar;
		ITimerMeter timer;

		IMeterRenderer meterRenderer;

		CountdownTimer time;

		#endregion //Properties

		#region Methods

		public MainPage() : base("MainPage")
		{
			Transition.OnTime = 3f;
		}

		private void Reset()
		{
			hitPoints = maxHP;
			mana = 0;
			lifeBar.Reset();
			manaBar.Reset();
			timer.Reset();
			time = new CountdownTimer();
		}

		public override void LoadContent()
		{
			base.LoadContent();

			//create the meter rectangles
			var lifebarRect = new Rectangle(
				(int)Resolution.TitleSafeArea.Left,
				(int)Resolution.TitleSafeArea.Top,
				512, 128);

			var manabarRect = new Rectangle(
				(int)Resolution.TitleSafeArea.Left,
				lifebarRect.Bottom,
				512, 128);

			var timerRect = new Rectangle(600,
				(int)Resolution.TitleSafeArea.Top,
				128, 128);

			//create the lifebar
			lifeBar = new LifeBar(maxHP, Content, "lifebarBorder.png", "lifebar.png", "lifebarGradient.png", lifebarRect);
			timer = new TimerMeter(maxTime, Content, "TimerBackground.png", "TimerMeter.png", "TimerGradient.png", timerRect);
			//manaBar = new SuperBar(maxMana, Content, "energybackground.png", "energymeter.png", "energygradient.png", manabarRect);
			manaBar = new SuperBar(maxMana, Content, "energymeter1.png", "energymetermask1.png", "energymetergradient1.png", manabarRect);
			meterRenderer = new MeterRenderer(Content, "MeterShader.fx");

			//add a stack of buttons for interacting with stuff
			var lifeButtonStack = new StackLayout(StackAlignment.Bottom)
			{
				Position = new Point(Resolution.TitleSafeArea.Left, Resolution.TitleSafeArea.Bottom),
				Horizontal = HorizontalAlignment.Left,
				Vertical = VerticalAlignment.Top
			};
			var hitButton = AddButton("Add Damage");
			hitButton.OnClick += HitButton_OnClick;
			lifeButtonStack.AddItem(hitButton);

			var healButton = AddButton("Heal");
			healButton.OnClick += HealButton_OnClick;
			lifeButtonStack.AddItem(healButton);

			var addManaButton = AddButton("Add Energy");
			addManaButton.OnClick += AddManaButton_OnClick;
			lifeButtonStack.AddItem(addManaButton);

			var spendManaButton = AddButton("Use Super");
			spendManaButton.OnClick += SpendManaButton_OnClick;
			lifeButtonStack.AddItem(spendManaButton);

			var nopeButton = AddButton("Super Nope");
			nopeButton.OnClick += NopeButton_OnClick;
			lifeButtonStack.AddItem(nopeButton);

			var resetTime = AddButton("ResetTime");
			resetTime.OnClick += ResetTime_OnClick;
			lifeButtonStack.AddItem(resetTime);

			AddItem(lifeButtonStack);

			Reset();
		}

		private void ResetTime_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			time.Start(maxTime);
			timer.Reset();
		}

		private void NopeButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			manaBar.Nope();
		}

		private void SpendManaButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			mana = 0;
			manaBar.UseEnergy();
		}

		private void AddManaButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			var damage = 10f;
			mana += damage;
			manaBar.AddEnergy(damage);
		}

		private void HealButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			var damage = 10f;
			hitPoints += damage;
			lifeBar.Heal(damage);
		}

		private void HitButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			var damage = 5f;
			hitPoints -= damage;
			lifeBar.AddDamage(damage);
		}

		private RelativeLayoutButton AddButton(string text)
		{
			var hitButton = new RelativeLayoutButton()
			{
				Size = new Vector2(256, 72),
				HasOutline = true,
				HasBackground = true,
				Horizontal = HorizontalAlignment.Left,
				Vertical = VerticalAlignment.Top
			};
			hitButton.AddItem(new Label(text, Content, FontSize.Small)
			{
				Horizontal = HorizontalAlignment.Center,
				Vertical = VerticalAlignment.Center
			});
			return hitButton;
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
			time.Update(gameTime);

			lifeBar.Update(gameTime);
			manaBar.Update(gameTime);
			timer.Update(time);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//draw the meters
			meterRenderer.Alpha = Transition.Alpha;
			meterRenderer.SpriteBatchBegin(ScreenManager.SpriteBatch, Resolution.TransformationMatrix());

			lifeBar.Draw(hitPoints, meterRenderer, ScreenManager.SpriteBatch);

			manaBar.Draw(mana, meterRenderer, ScreenManager.SpriteBatch);

			timer.Draw(time.RemainingTime, meterRenderer, ScreenManager.SpriteBatch);

			ScreenManager.SpriteBatch.End();
		}

		#endregion //Methods
	}
}
