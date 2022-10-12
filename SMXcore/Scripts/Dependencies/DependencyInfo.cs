using ICSharpCode.WpfDesign.XamlDom;
using System.Xml;
using XMLData;
using XMLData.Exceptions;

public class DependencyInfo
{
    public string ModName { get; internal set; }
    public string Version { get; internal set; }
    public ModDependent DependentMod { get; internal set; }

    public static DependencyInfo FromXml(XmlNode node)
    {
        if(node.NodeType != XmlNodeType.Element)
        {
            throw new UnexpectedElementException("Unknown node \"" + node.NodeType.ToString() + "\" found while parsing Dependencies", ((IXmlLineInfo)node).LineNumber);
        }

        if(node.Name != "Dependency")
        {
            throw new UnexpectedElementException("Unknown element \"" + node.Name + "\" found while parsing Dependencies", ((IXmlLineInfo)node).LineNumber);
        }

        DependencyInfo dependencyInfo = new DependencyInfo();

        PositionXmlElement positionXmlElement = (PositionXmlElement)node;

        dependencyInfo.ModName = ParserUtils.ParseStringAttribute(positionXmlElement, "mod", _mandatory: true);
        dependencyInfo.Version = ParserUtils.ParseStringAttribute(positionXmlElement, "version", _mandatory: true);

        return dependencyInfo;

    }
}
