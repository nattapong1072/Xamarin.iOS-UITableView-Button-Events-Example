using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

/**
 * This project serves as an example of handling and invoking tap events of UIButtons within custom UITableViewCell implementations.
 * In this project, a custom UITableViewCell implementation contains a UIButton that, when tapped, executes a function within the View Controller.
 * The process is done by setting up a "stack" of events between the UIButton and the View Controller, and connecting them with event handlers.
 * Generally the connections work like so: View Controller <--> UITableViewSource <--> UITableViewCell <--> UIButton
 * 
 * This pattern is not specific to UIButtons, but rather it can be implemented for any event-based element added within custom UITableViewCell implementations.
 * 
 * This example project was created by William Thomas - http://www.willseph.com/
 * Licensed under the WTFPL Public License v2 (WTFPL-2.0) - https://www.tldrlegal.com/l/wtfpl
 **/

namespace TableButtonExample
{
	public class MainViewController : UIViewController
	{
		private UITableView AnimalTableView;

		public MainViewController () : base(null, null)
		{
			this.Title = "Table Button Example";
			this.View.BackgroundColor = UIColor.White;

			// Creating the table view and adding it to the ViewController's view.
			AnimalTableView = new UITableView (new CGRect(), UITableViewStyle.Plain) {
				SeparatorStyle = UITableViewCellSeparatorStyle.None,
				AllowsSelection = false
			};
			View.AddSubview (AnimalTableView);

			// Creating an example list of animals.
			List<Animal> ExampleAnimals = new List<Animal> () {
				new Animal("Bird", "Tweet"),
				new Animal("Cat", "Meow"),
				new Animal("Cow", "Moo"),
				new Animal("Dog", "Woof"),
				new Animal("Horse", "Neigh"),
				new Animal("Mouse", "Squeak")
			};

			// Applying a new AnimalTableViewSource to the table view and setting the example data.
			AnimalTableViewSource TableViewSource = new AnimalTableViewSource ();
			AnimalTableView.Source = TableViewSource;
			TableViewSource.SetData (ExampleAnimals);
			AnimalTableView.ReloadData ();

			// Creating the top-level event handler for the AnimalTableViewSource's SpeakButtonTapped event.
			TableViewSource.SpeakButtonTapped -= OnSpeakButtonTapped;
			TableViewSource.SpeakButtonTapped += OnSpeakButtonTapped;
		}

		void OnSpeakButtonTapped (object sender, Animal e)
		{
			// This is the event handler for the AnimalTableViewSource's SpeakButtonTapped event.
			// We no longer have to "bubble up" the event since we will be handling its final functionality here.

			new UIAlertView (
				string.Format ("The {0} says", e.Name),
				string.Format ("{0}!", e.Sound),
				null,
				"Close").Show ();
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();
			if (AnimalTableView != null)
				AnimalTableView.Frame = View.Bounds;
		}
	}

	public class AnimalTableViewSource : UITableViewSource
	{
		/// <summary>
		/// The reuse identifier used for recycling cells.
		/// </summary>
		public const string REUSE_IDENTIFIER = @"AnimalTable";

		/// <summary>
		/// Occurs when a "Speak" button is tapped within one of the cells.
		/// </summary>
		public event EventHandler<Animal> SpeakButtonTapped;

		private List<Animal> Animals;

		public AnimalTableViewSource() : base()
		{
		}

		public void SetData(List<Animal> Animals)
		{
			this.Animals = Animals;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return Animals == null ? 0 : Animals.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			if (Animals == null || Animals.Count < 1)
				return null;

			Animal CellAnimal = Animals [indexPath.Row];

			AnimalTableViewCell Cell = tableView.DequeueReusableCell (REUSE_IDENTIFIER) as AnimalTableViewCell;
			if (Cell == null) {
				Cell = new AnimalTableViewCell (REUSE_IDENTIFIER);

				// Here is where the OnClick event handler is set for the cell.
				// Notice that the event handler only needs to be applied on the creation of a brand new cell.
				// Recycled cells can use their existing event handlers.
				Cell.SpeakButtonTapped -= OnCellSpeakButtonTapped;
				Cell.SpeakButtonTapped += OnCellSpeakButtonTapped;
			}

			Cell.SetAnimal (CellAnimal, (indexPath.Row%2 == 0 ? Parity.Even : Parity.Odd));
			return Cell;
		}

		void OnCellSpeakButtonTapped (object sender, Animal e)
		{
			// This is the event handler for the AnimalTableViewCell's SpeakButtonTapped event.
			// In this handler, we will invoke the AnimalTableViewSource's SpeakButtonTapped event causing it to "bubble up" into the view controller.

			if (SpeakButtonTapped != null)
				SpeakButtonTapped (sender, e);
		}

		public override nfloat GetHeightForRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return AnimalTableViewCell.HEIGHT;
		}
	}

	public class AnimalTableViewCell : UITableViewCell
	{
		/// <summary>
		/// The constant height of each cell.
		/// </summary>
		public static readonly nfloat HEIGHT = 50;

		/// <summary>
		/// The current Animal data object of this cell.
		/// </summary>
		public Animal CurrentAnimal { get; private set; }

		/// <summary>
		/// The label which displays the name of the animal.
		/// </summary>
		private UILabel NameLabel;

		/// <summary>
		/// The button that will make the cell "speak".
		/// </summary>
		private UIButton SpeakButton;

		/// <summary>
		/// Occurs when the "Speak" button is tapped.
		/// </summary>
		public event EventHandler<Animal> SpeakButtonTapped;

		private static readonly UIColor EvenBGColor = UIColor.FromRGB(240,240,240);
		private static readonly UIColor OddBGColor = UIColor.FromRGB(230,230,230);

		public AnimalTableViewCell(string ReuseIdentifier) : base(UITableViewCellStyle.Default, ReuseIdentifier)
		{
			// Creating and adding the name label to the cell's content view.
			this.NameLabel = new UILabel () {
				TextColor = UIColor.Black,
				TextAlignment = UITextAlignment.Left
			};
			this.ContentView.AddSubview (this.NameLabel);

			// Creating and adding the Speak button to the cell's content view.
			this.SpeakButton = new UIButton (UIButtonType.RoundedRect);
			this.ContentView.AddSubview (this.SpeakButton);
			this.SpeakButton.SetTitle ("Speak", UIControlState.Normal);
			this.SpeakButton.SetNeedsLayout ();

			// Here is where the lowest-level event handler is applied to the speak button.
			SpeakButton.TouchUpInside -= OnSpeakButtonTapped;
			SpeakButton.TouchUpInside += OnSpeakButtonTapped;
		}

		void OnSpeakButtonTapped (object sender, EventArgs e)
		{
			// This is the event handler for the actual UIButton TouchUpInside event.
			// In this handler, we will invoke this cell's SpeakButtonTapped event to "bubble up" to the TableViewSource.
			if (SpeakButtonTapped != null)
				SpeakButtonTapped (this, CurrentAnimal);
		}

		public void SetAnimal(Animal Animal, Parity Parity)
		{
			this.CurrentAnimal = Animal;
			this.NameLabel.Text = Animal.Name;

			switch (Parity) {
				case Parity.Even:
					BackgroundColor = EvenBGColor;
					break;
				case Parity.Odd:
					BackgroundColor = OddBGColor;
					break;
			}
		}

		public override void LayoutSubviews()
		{
			if (NameLabel != null) 
				NameLabel.Frame = new CGRect (10, 10, ContentView.Bounds.Width - 20, ContentView.Bounds.Height - 20);
			
			if (SpeakButton != null) {
				CGSize ButtonSize = new CGSize (60, 30);
				SpeakButton.Frame = new CGRect (new CGPoint (ContentView.Bounds.Width - ButtonSize.Width - 10, ContentView.Bounds.Height / 2.0f - ButtonSize.Height / 2.0f), ButtonSize);
			}
		}
	}

	/// <summary>
	/// An example data model class. Nothing fancy.
	/// </summary>
	public class Animal
	{
		public string Name { get; private set; }
		public string Sound { get; private set; }

		public Animal(string Name, string Sound)
		{
			this.Name = Name;
			this.Sound = Sound;
		}
	}

	/// <summary>
	/// A basic even/odd parity enumeration.
	/// </summary>
	public enum Parity
	{
		Even,
		Odd
	}
}