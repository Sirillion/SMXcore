using ICSharpCode.WpfDesign.XamlDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public static class DependencyInfoLoader
{
    public static List<DependencyInfo> ParseXml(string filename, string content)
    {
		PositionXmlDocument positionXmlDocument = new PositionXmlDocument();
		try
		{
			using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(content ?? "")))
			{
				positionXmlDocument.Load(XmlReader.Create(stream));
			}
		}
		catch (XmlException e)
		{
			Log.Error("Failed parsing " + filename + ":");
			Log.Exception(e);
			return null;
		}

        XmlElement documentElement = positionXmlDocument.DocumentElement;
        List<DependencyInfo> list = new List<DependencyInfo>();
        foreach (XmlNode childNode in documentElement.ChildNodes)
        {
            switch (childNode.NodeType)
            {
                case XmlNodeType.Element:
                    {
                        string name = childNode.Name;
                        if (name != null && name == "Dependencies")
                        {
                            list.AddRange(ParseDependencies(filename, childNode));
                        }
                        else
                        {
                            Log.Warning($"Unknown element found: {childNode.Name} (file {filename}, line {((IXmlLineInfo)childNode).LineNumber})");
                        }

                        break;
                    }
                default:
                    Log.Error("Unexpected XML node: " + childNode.NodeType.ToString() + " at line " + ((IXmlLineInfo)childNode).LineNumber);
                    break;
                case XmlNodeType.Comment:
                    break;
            }
        }

        return list;
    }

    private static List<DependencyInfo> ParseDependencies(string filename, XmlNode node)
    {
        List<DependencyInfo> list = new List<DependencyInfo>();

        foreach (XmlNode childNode in node.ChildNodes)
        {
            switch (childNode.NodeType)
            {
                case XmlNodeType.Element:
                    {
                        string name = childNode.Name;
                        if (name != null && name == "Dependency")
                        {
                            DependencyInfo dependencyInfo = DependencyInfo.FromXml(childNode);
                            if (dependencyInfo != null)
                            {
                                list.Add(dependencyInfo);
                            }
                        }
                        else
                        {
                            Log.Warning($"Unknown element found: {childNode.Name} (file {filename}, line {((IXmlLineInfo)childNode).LineNumber})");
                        }

                        break;
                    }
                default:
                    Log.Error("Unexpected XML node: " + childNode.NodeType.ToString() + " at line " + ((IXmlLineInfo)childNode).LineNumber);
                    break;
                case XmlNodeType.Comment:
                    break;
            }
        }
        return list;
    }
}
