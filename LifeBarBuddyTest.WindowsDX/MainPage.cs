using FilenameBuddy;
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

		float hitPoints;
		float mana;

		ILifeBar lifeBar;
		ISuperBar manaBar;

		IMeterRenderer meterRenderer;

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

			//create the lifebar
			lifeBar = new LifeBar(maxHP, Content, "lifebarBorder.png", "lifebar.png", "lifebarGradient.png", lifebarRect);
			manaBar = new SuperBar(maxMana, Content, "energybackground.png", "energymeter.png", "energygradient.png", manabarRect);
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
			spendManaButton.OnClick += SpendManaButton_OnClick; ;
			lifeButtonStack.AddItem(spendManaButton);

			AddItem(lifeButtonStack);

			Reset();
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

			lifeBar.Update(gameTime);
			manaBar.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//draw the meters
			meterRenderer.Alpha = Transition.Alpha;
			meterRenderer.SpriteBatchBegin(ScreenManager.SpriteBatch, Resolution.TransformationMatrix());

			lifeBar.Draw(hitPoints, meterRenderer, ScreenManager.SpriteBatch);

			manaBar.Draw(mana, meterRenderer, ScreenManager.SpriteBatch);

			ScreenManager.SpriteBatch.End();
		}

		#endregion //Methods
	}
}
