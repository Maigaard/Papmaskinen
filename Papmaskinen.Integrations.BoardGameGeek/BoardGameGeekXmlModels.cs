/* 
    Licensed under the Apache License, Version 2.0
    
    http://www.apache.org/licenses/LICENSE-2.0
    */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Papmaskinen.Integrations.BoardGameGeek.Models
{
    [XmlRoot(ElementName = "name")]
    public class Name
    {
        [XmlAttribute(AttributeName = "type")]
        public string? Type { get; set; }
        [XmlAttribute(AttributeName = "sortindex")]
        public string? Sortindex { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "yearpublished")]
    public class Yearpublished
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "minplayers")]
    public class Minplayers
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "maxplayers")]
    public class Maxplayers
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "result")]
    public class Result
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
        [XmlAttribute(AttributeName = "numvotes")]
        public string? Numvotes { get; set; }
        [XmlAttribute(AttributeName = "level")]
        public string? Level { get; set; }
    }

    [XmlRoot(ElementName = "results")]
    public class Results
    {
        [XmlElement(ElementName = "result")]
        public IEnumerable<Result>? Result { get; set; }
        [XmlAttribute(AttributeName = "numplayers")]
        public string? Numplayers { get; set; }
    }

    [XmlRoot(ElementName = "poll")]
    public class Poll
    {
        [XmlElement(ElementName = "results")]
        public IEnumerable<Results>? Results { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string? Name { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string? Title { get; set; }
        [XmlAttribute(AttributeName = "totalvotes")]
        public string? Totalvotes { get; set; }
    }

    [XmlRoot(ElementName = "playingtime")]
    public class Playingtime
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "minplaytime")]
    public class Minplaytime
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "maxplaytime")]
    public class Maxplaytime
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "minage")]
    public class Minage
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "link")]
    public class Link
    {
        [XmlAttribute(AttributeName = "type")]
        public string? Type { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string? Id { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
        [XmlAttribute(AttributeName = "inbound")]
        public string? Inbound { get; set; }
    }

    [XmlRoot(ElementName = "usersrated")]
    public class Usersrated
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "average")]
    public class Average
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "bayesaverage")]
    public class Bayesaverage
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "rank")]
    public class Rank
    {
        [XmlAttribute(AttributeName = "type")]
        public string? Type { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string? Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string? Name { get; set; }
        [XmlAttribute(AttributeName = "friendlyname")]
        public string? Friendlyname { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
        [XmlAttribute(AttributeName = "bayesaverage")]
        public string? Bayesaverage { get; set; }
    }

    [XmlRoot(ElementName = "ranks")]
    public class Ranks
    {
        [XmlElement(ElementName = "rank")]
        public Rank? Rank { get; set; }
    }

    [XmlRoot(ElementName = "stddev")]
    public class Stddev
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "median")]
    public class Median
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "owned")]
    public class Owned
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "trading")]
    public class Trading
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "wanting")]
    public class Wanting
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "wishing")]
    public class Wishing
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "numcomments")]
    public class Numcomments
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "numweights")]
    public class Numweights
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "averageweight")]
    public class Averageweight
    {
        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }
    }

    [XmlRoot(ElementName = "ratings")]
    public class Ratings
    {
        [XmlElement(ElementName = "usersrated")]
        public Usersrated? Usersrated { get; set; }
        [XmlElement(ElementName = "average")]
        public Average? Average { get; set; }
        [XmlElement(ElementName = "bayesaverage")]
        public Bayesaverage? Bayesaverage { get; set; }
        [XmlElement(ElementName = "ranks")]
        public Ranks? Ranks { get; set; }
        [XmlElement(ElementName = "stddev")]
        public Stddev? Stddev { get; set; }
        [XmlElement(ElementName = "median")]
        public Median? Median { get; set; }
        [XmlElement(ElementName = "owned")]
        public Owned? Owned { get; set; }
        [XmlElement(ElementName = "trading")]
        public Trading? Trading { get; set; }
        [XmlElement(ElementName = "wanting")]
        public Wanting? Wanting { get; set; }
        [XmlElement(ElementName = "wishing")]
        public Wishing? Wishing { get; set; }
        [XmlElement(ElementName = "numcomments")]
        public Numcomments? Numcomments { get; set; }
        [XmlElement(ElementName = "numweights")]
        public Numweights? Numweights { get; set; }
        [XmlElement(ElementName = "averageweight")]
        public Averageweight? Averageweight { get; set; }
    }

    [XmlRoot(ElementName = "statistics")]
    public class Statistics
    {
        [XmlElement(ElementName = "ratings")]
        public Ratings? Ratings { get; set; }
        [XmlAttribute(AttributeName = "page")]
        public string? Page { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "thumbnail")]
        public string? Thumbnail { get; set; }
        [XmlElement(ElementName = "image")]
        public string? Image { get; set; }
        [XmlElement(ElementName = "name")]
        public IEnumerable<Name>? Name { get; set; }
        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }
        [XmlElement(ElementName = "yearpublished")]
        public Yearpublished? Yearpublished { get; set; }
        [XmlElement(ElementName = "minplayers")]
        public Minplayers? Minplayers { get; set; }
        [XmlElement(ElementName = "maxplayers")]
        public Maxplayers? Maxplayers { get; set; }
        [XmlElement(ElementName = "poll")]
        public IEnumerable<Poll>? Poll { get; set; }
        [XmlElement(ElementName = "playingtime")]
        public Playingtime? Playingtime { get; set; }
        [XmlElement(ElementName = "minplaytime")]
        public Minplaytime? Minplaytime { get; set; }
        [XmlElement(ElementName = "maxplaytime")]
        public Maxplaytime? Maxplaytime { get; set; }
        [XmlElement(ElementName = "minage")]
        public Minage? Minage { get; set; }
        [XmlElement(ElementName = "link")]
        public IEnumerable<Link>? Link { get; set; }
        [XmlElement(ElementName = "statistics")]
        public Statistics? Statistics { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string? Type { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string? Id { get; set; }
    }

    [XmlRoot(ElementName = "items")]
    public class ThingResultSet
    {
        [XmlElement(ElementName = "item")]
        public IEnumerable<Item>? Items { get; set; }
        [XmlAttribute(AttributeName = "termsofuse")]
        public string? Termsofuse { get; set; }
    }
}
