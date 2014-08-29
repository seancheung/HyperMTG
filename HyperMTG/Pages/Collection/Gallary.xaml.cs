using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace HyperMTG.Pages
{
	/// <summary>
	///     Interaction logic for Gallary.xaml
	/// </summary>
	public partial class Gallary
	{
		/// <summary>
		///     Represents whether the book is open or not.
		/// </summary>
		private bool _isBookOpen;

		public Gallary()
		{
			InitializeComponent();
		}

		/// <summary>
		///     Opens the 3D book.
		/// </summary>
		/// <param flavor="durationSeconds">Time in seconds that the animation will take.</param>
		private void OpenBook(double durationSeconds)
		{
			// Transform3D_LeftRotation
			var rot = (RotateTransform3D) TryFindResource("Transform3D_LeftRotation");
			var da = new DoubleAnimation(15, new Duration(TimeSpan.FromSeconds(durationSeconds)));
			da.DecelerationRatio = 1;
			rot.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

			// Transform3D_RightRotation
			rot = (RotateTransform3D) TryFindResource("Transform3D_RightRotation");
			da = new DoubleAnimation(-15, new Duration(TimeSpan.FromSeconds(durationSeconds)));
			rot.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

			// Transform3D_SpineRotation
			rot = (RotateTransform3D) TryFindResource("Transform3D_SpineRotation");
			da = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.8333*durationSeconds)));
			rot.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

			// Transform3D_SpineCoverTranslation
			var trans = (TranslateTransform3D) TryFindResource("Transform3D_SpineCoverTranslation");
			da = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.8333*durationSeconds)));
			trans.BeginAnimation(TranslateTransform3D.OffsetXProperty, da);

			// _Main3D.Camera
			var pa = new Point3DAnimation(new Point3D(0, -2.5, 6.5),
				new Duration(TimeSpan.FromSeconds(durationSeconds)));
			pa.AccelerationRatio = 0.5;
			pa.DecelerationRatio = 0.5;
			_Main3D.Camera.BeginAnimation(ProjectionCamera.PositionProperty, pa);

			// Now the book is open.
			_isBookOpen = true;
		}

		/// <summary>
		///     Closes the 3D book.
		/// </summary>
		/// <param flavor="durationSeconds">Time in seconds that the animation will take.</param>
		private void CloseBook(double durationSeconds)
		{
			// Transform3D_LeftRotation
			var rot = (RotateTransform3D) TryFindResource("Transform3D_LeftRotation");
			var da = new DoubleAnimation(180, new Duration(TimeSpan.FromSeconds(durationSeconds)));
			da.DecelerationRatio = 1;
			rot.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

			// Transform3D_RightRotation
			rot = (RotateTransform3D) TryFindResource("Transform3D_RightRotation");
			da = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(durationSeconds)));
			rot.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

			// Transform3D_SpineRotation
			rot = (RotateTransform3D) TryFindResource("Transform3D_SpineRotation");
			da = new DoubleAnimation(90, new Duration(TimeSpan.FromSeconds(0.8333*durationSeconds)));
			rot.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

			// Transform3D_SpineCoverTranslation
			var trans = (TranslateTransform3D) TryFindResource("Transform3D_SpineCoverTranslation");
			da = new DoubleAnimation(-0.125, new Duration(TimeSpan.FromSeconds(0.8333*durationSeconds)));
			trans.BeginAnimation(TranslateTransform3D.OffsetXProperty, da);

			// _Main3D.Camera
			var pa = new Point3DAnimation(new Point3D(0.72, -2.5, 6.5),
				new Duration(TimeSpan.FromSeconds(durationSeconds)));
			pa.AccelerationRatio = 0.5;
			pa.DecelerationRatio = 0.5;
			_Main3D.Camera.BeginAnimation(ProjectionCamera.PositionProperty, pa);

			// Now the book is closed.
			_isBookOpen = false;
		}

		private void Gallary_OnLoaded(object sender, RoutedEventArgs e)
		{
			CloseBook(0); // Book starts closed

			// Make book fade in
			var da = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(2)));
			da.DecelerationRatio = 1;
			_Main3D.BeginAnimation(OpacityProperty, da);
		}

		private void UIElement3D_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (_isBookOpen)
				CloseBook(1.5);
			else
				OpenBook(1.5);
		}
	}
}