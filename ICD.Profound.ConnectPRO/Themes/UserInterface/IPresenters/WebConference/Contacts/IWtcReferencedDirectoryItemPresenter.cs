using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts
{
	public interface IWtcReferencedDirectoryItemPresenter : IUiPresenter<IWtcReferencedDirectoryItemView>
	{
		event EventHandler OnPressed;

		DirectoryItem DirectoryItem { get; set; }

		bool Selected { get; set; }
	}

	public struct DirectoryItem
	{
		public enum eModelType
		{
			Folder,
			Contact
		}

		private readonly eModelType m_ModelType;
		private readonly object m_Model;

		public eModelType ModelType { get { return m_ModelType; } }

		public object Model { get { return m_Model; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presenterType"></param>
		/// <param name="model"></param>
		public DirectoryItem(IContact model)
		{
			m_ModelType = eModelType.Contact;
			m_Model = model;
		}

		public DirectoryItem(IDirectoryFolder model)
		{
			m_ModelType = eModelType.Folder;
			m_Model = model;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DirectoryItem))
				return false;

			var directoryItem = (DirectoryItem) obj;
			return ModelType == directoryItem.ModelType && Model == directoryItem.Model;
		}

		public static bool operator ==(DirectoryItem a, DirectoryItem b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(DirectoryItem a, DirectoryItem b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			int hash = 13;
			hash = hash * 7 + m_ModelType.GetHashCode();
			hash = hash * 7 + m_Model.GetHashCode();
			return hash;
		}
	}
}