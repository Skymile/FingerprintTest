﻿using System.Windows;
using System.Windows.Controls;

using FDB.Biometrics;
using FDB.Networking.Users;

namespace FDB.View
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		public LoginWindow(ref Userbase userbase, FingerprintScanner scanner)
		{
			InitializeComponent();
			this._Userbase = userbase;
			this._Scanner = scanner;
		}

		private void WindowButtonLogin_Click(object sender, RoutedEventArgs e)
		{
			if (IsUsernameNull)
			{
				WindowErrorLabel.Content = ErrorUsernameInvalid;
				return;
			}
			if (!UsernameExists(WindowBoxLoginUsername))
			{
				WindowErrorLabel.Content = ErrorUsernameInvalid;
				return;
			}
			ValidLogin(this.WindowBoxLoginUsername.Text, this.WindowBoxLoginPassword.Text);
		}

		private bool IsUsernameNull => this.WindowBoxLoginUsername.Text == null;
		private bool IsPasswordNull => this.WindowBoxLoginPassword.Text == null;
		private bool IsFingerprintNull => this._Fingerprint == null;

		private const string ErrorUsernameInvalid = "Type in valid username";
		private const string ErrorPasswordInvalid = "Type in valid password";
		private const string ErrorFingerprintInvalid = "Fingerprint needed";
		private const string UsernameIncorrect = "Incorrect login";

		private bool UsernameExists(TextBox box) => 
			this._Userbase.Find(i => i.GetElement().Username.CompareTo(box.Text) == 0) == null;

		private void WindowButtonCapture_Click(object sender, RoutedEventArgs e)
		{
			if (_Scanner == null)
				new Message(Message.Error.DeviceNotFound);
			else
				this._Fingerprint = _Scanner.CaptureFingerprintData();
		}

		private void ValidLogin(string username, string password)
		{
			if (IsFingerprintNull)
			{
				var match = this._Userbase.Find(
					i => i.GetElement().Username == username && i.GetElement().Password == password
				);

				WindowErrorLabel.Content = match == null ? "null" : "matched";
			}
			else
			{
				int result = _Fingerprint.CompareFmd(this._Userbase[0].GetElement().Finger);

				WindowErrorLabel.Content = result.ToString();
			}
		}

		private Userbase _Userbase;
		private FingerprintScanner _Scanner;

		private Fingerprint _Fingerprint;
	}
}
