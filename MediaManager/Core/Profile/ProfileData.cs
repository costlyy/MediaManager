using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MediaManager.Logging;

namespace MediaManager.Core.Profile
{
	public abstract class ProfileData<T>
	{
		public string Path { get; }

		public ProfileData(string path)
		{
			if (path.Length <= 0) return;

			Path = path;
		}

		public virtual ProfileData<T> Import()
		{
			LogWriter.Write($"{typeof(T)} # No import function defined.");
			return null;
		}

		public virtual void Export()
		{
			LogWriter.Write($"{typeof(T)} # No export function defined.");
		}

		protected XmlWriter StartExport(string name)
		{
			var settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = ("    "),
				CloseOutput = true,
			};

			XmlWriter writer = XmlWriter.Create(string.Format($"{Path}{@"\"}{name}{".xml"}"), settings);
			writer.WriteStartDocument();
			writer.WriteComment("This is an automatically generated file, changes made may be overwritten");
			writer.WriteStartElement(name);

			return writer;
		}

		protected bool StopExport(XmlWriter writer)
		{
			if (writer == null)
			{
				LogWriter.Write($"{typeof(T)} # StopExport was provided with a null writer, cannot finish exporting data.", DebugPriority.High, true);
				return false;
			}

			writer.WriteEndElement();

			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();

			return true;
		}

		protected bool StartImport(string name, out XmlReader reader)
		{
			try
			{
				reader = XmlReader.Create(string.Format($"{Path}{@"\"}{name}{".xml"}"));
				reader.ReadStartElement(name);
				return true;
			}
			catch (FileNotFoundException e)
			{
				reader = null;
				LogWriter.Write($"{typeof(T)} # StartImport could not find file: {name}. Full exception:\n\n{e}", DebugPriority.High);
				return false;
			}
		}

		protected bool StopImport(XmlReader reader)
		{
			if (reader == null)
			{
				LogWriter.Write($"{typeof(T)} # StopImport was provided with a null reader, cannot finish importing data.", DebugPriority.High, true);
				return false;
			}

			reader.Close();
			reader = null;
			return true;
		}
	}
}
