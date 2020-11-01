﻿namespace BusinessRules.Utils
{
    public class HtmlToText
    {
        public static string Preprocessing(string myText)
        {
            //Special formatting characters for magazine systems
            myText = myText.Replace("&QL;", "");
            myText = myText.Replace("&QC;", "");
            myText = myText.Replace("&QR;", "");
            myText = myText.Replace("&TL;", "");
            myText = myText.Replace("&UR;", "");
            myText = myText.Replace("&LR;", "");
            myText = myText.Replace("&HT;", "");
            myText = myText.Replace("&AMP;", "&amp;");

            //Special tags for Linguistic Data Consortium (LDC)
            myText = myText.Replace("&Cx1f;", "[Cx1f]");
            myText = myText.Replace("&Cx11;", "[Cx11]");
            myText = myText.Replace("&Cx13;", "[Cx13]");

            //Special characters in XML
            myText = myText.Replace("&amp;", "&");
            myText = myText.Replace("&#38;", "&");
            myText = myText.Replace("&#39;", "'");
            //myText = myText.Replace("&", "&amp;");
            //myText = myText.Replace("&#38;", "&amp;");
            //myText = myText.Replace("'", "&#39;");

            myText = myText.Replace("", "");
            myText = myText.Replace("", "");

            myText = myText.Replace("<b>", "");
            myText = myText.Replace("</b>", "");
            myText = myText.Replace("<br>  ", "\r\n");
            myText = myText.Replace("|", " ");
            myText = myText.Replace("&ndash;", ""); myText = myText.Replace("&#8211;", "");
            myText = myText.Replace("&mdash;", ""); myText = myText.Replace("&#8212;", "");
            myText = myText.Replace("&iexcl;", ""); myText = myText.Replace("&#161;", "");
            myText = myText.Replace("&iquest;", ""); myText = myText.Replace("&#191;", "");
            myText = myText.Replace("&ldquo;", ""); myText = myText.Replace("&#8220;", "");
            myText = myText.Replace("&rdquo;", ""); myText = myText.Replace("&#8221;", "");
            myText = myText.Replace("&lsquo;", ""); myText = myText.Replace("&#8216;", "");
            myText = myText.Replace("&rsquo;", ""); myText = myText.Replace("&#8217;", "");
            myText = myText.Replace("&laquo;", ""); myText = myText.Replace("&#171;", "");
            myText = myText.Replace("&raquo;", ""); myText = myText.Replace("&#187;", "");
            myText = myText.Replace("&nbsp;", " "); myText = myText.Replace("&#160;", " ");

            myText = myText.Replace("&cent;", "¢"); myText = myText.Replace("&#162;", "¢");
            myText = myText.Replace("&copy;", ""); myText = myText.Replace("&#169;", "");
            myText = myText.Replace("&divide;", ""); myText = myText.Replace("&#247;", "");
            myText = myText.Replace("&micro;", "µ"); myText = myText.Replace("&#181;", "µ");
            myText = myText.Replace("&middot;", ""); myText = myText.Replace("&#183;", "");
            myText = myText.Replace("&para;", ""); myText = myText.Replace("&#182;", "");
            myText = myText.Replace("&plusmn;", "±"); myText = myText.Replace("&#177;", "±");
            myText = myText.Replace("&euro;", "€"); myText = myText.Replace("&#8364;", "€");
            myText = myText.Replace("&pound;", "£"); myText = myText.Replace("&#163;", "£");
            myText = myText.Replace("&reg;", ""); myText = myText.Replace("&#174;", "");
            myText = myText.Replace("&sect;", "§"); myText = myText.Replace("&#167;", "§");
            myText = myText.Replace("&trade;", ""); myText = myText.Replace("&#153;", "");
            myText = myText.Replace("&yen;", "¥"); myText = myText.Replace("&#165;", "¥");
            myText = myText.Replace("&aacute;", "a"); myText = myText.Replace("&#225;", "a");
            myText = myText.Replace("&Aacute;", "a"); myText = myText.Replace("&#193;", "a");
            myText = myText.Replace("&agrave;", "à"); myText = myText.Replace("&#224;", "à");
            myText = myText.Replace("&Agrave;", "À"); myText = myText.Replace("&#192;", "À");
            myText = myText.Replace("&acirc;", "â"); myText = myText.Replace("&#226;", "â");
            myText = myText.Replace("&Acirc;", "Â"); myText = myText.Replace("&#194;", "Â");
            myText = myText.Replace("&aring;", "å"); myText = myText.Replace("&#229;", "å");
            myText = myText.Replace("&Aring;", "Å"); myText = myText.Replace("&#197;", "Å");
            myText = myText.Replace("&atilde;", "ã"); myText = myText.Replace("&#227;", "ã");
            myText = myText.Replace("&Atilde;", "Ã"); myText = myText.Replace("&#195;", "Ã");
            myText = myText.Replace("&auml;", "ä"); myText = myText.Replace("&#228;", "ä");
            myText = myText.Replace("&Auml;", "Ä"); myText = myText.Replace("&#196;", "Ä");
            myText = myText.Replace("&aelig;", "æ"); myText = myText.Replace("&#230;", "æ");
            myText = myText.Replace("&AElig;", "Æ"); myText = myText.Replace("&#198;", "Æ");
            myText = myText.Replace("&ccedil;", "ç"); myText = myText.Replace("&#231;", "ç");
            myText = myText.Replace("&Ccedil;", "Ç"); myText = myText.Replace("&#199;", "Ç");
            myText = myText.Replace("&eacute;", "e"); myText = myText.Replace("&#233;", "e");
            myText = myText.Replace("&Eacute;", "e"); myText = myText.Replace("&#201;", "e");
            myText = myText.Replace("&egrave;", "è"); myText = myText.Replace("&#232;", "è");
            myText = myText.Replace("&Egrave;", "È"); myText = myText.Replace("&#200;", "È");
            myText = myText.Replace("&ecirc;", "ê"); myText = myText.Replace("&#234;", "ê");
            myText = myText.Replace("&Ecirc;", "Ê"); myText = myText.Replace("&#202;", "Ê");
            myText = myText.Replace("&euml;", "ë"); myText = myText.Replace("&#235;", "ë");
            myText = myText.Replace("&Euml;", "Ë"); myText = myText.Replace("&#203;", "Ë");
            myText = myText.Replace("&iacute;", "i"); myText = myText.Replace("&#237;", "i");
            myText = myText.Replace("&Iacute;", "i"); myText = myText.Replace("&#205;", "i");
            myText = myText.Replace("&igrave;", "ì"); myText = myText.Replace("&#236;", "ì");
            myText = myText.Replace("&Igrave;", "Ì"); myText = myText.Replace("&#204;", "Ì");
            myText = myText.Replace("&icirc;", "î"); myText = myText.Replace("&#238;", "î");
            myText = myText.Replace("&Icirc;", "Î"); myText = myText.Replace("&#206;", "Î");
            myText = myText.Replace("&iuml;", "ï"); myText = myText.Replace("&#239;", "ï");
            myText = myText.Replace("&Iuml;", "Ï"); myText = myText.Replace("&#207;", "Ï");
            myText = myText.Replace("&ntilde;", "ñ"); myText = myText.Replace("&#241;", "ñ");
            myText = myText.Replace("&Ntilde;", "ñ"); myText = myText.Replace("&#209;", "ñ");
            myText = myText.Replace("&oacute;", "o"); myText = myText.Replace("&#243;", "o");
            myText = myText.Replace("&Oacute;", "o"); myText = myText.Replace("&#211;", "o");
            myText = myText.Replace("&ograve;", "ò"); myText = myText.Replace("&#242;", "ò");
            myText = myText.Replace("&Ograve;", "Ò"); myText = myText.Replace("&#210;", "Ò");
            myText = myText.Replace("&ocirc;", "ô"); myText = myText.Replace("&#244;", "ô");
            myText = myText.Replace("&Ocirc;", "Ô"); myText = myText.Replace("&#212;", "Ô");
            myText = myText.Replace("&oslash;", "ø"); myText = myText.Replace("&#248;", "ø");
            myText = myText.Replace("&Oslash;", "Ø"); myText = myText.Replace("&#216;", "Ø");
            myText = myText.Replace("&otilde;", "õ"); myText = myText.Replace("&#245;", "õ");
            myText = myText.Replace("&Otilde;", "Õ"); myText = myText.Replace("&#213;", "Õ");
            myText = myText.Replace("&ouml;", "ö"); myText = myText.Replace("&#246;", "ö");
            myText = myText.Replace("&Ouml;", "Ö"); myText = myText.Replace("&#214;", "Ö");
            myText = myText.Replace("&szlig;", "ß"); myText = myText.Replace("&#223;", "ß");
            myText = myText.Replace("&uacute;", "u"); myText = myText.Replace("&#250;", "u");
            myText = myText.Replace("&Uacute;", "u"); myText = myText.Replace("&#218;", "u");
            myText = myText.Replace("&ugrave;", "ù"); myText = myText.Replace("&#249;", "ù");
            myText = myText.Replace("&Ugrave;", "Ù"); myText = myText.Replace("&#217;", "Ù");
            myText = myText.Replace("&ucirc;", "û"); myText = myText.Replace("&#251;", "û");
            myText = myText.Replace("&Ucirc;", "Û"); myText = myText.Replace("&#219;", "Û");
            myText = myText.Replace("&uuml;", "u"); myText = myText.Replace("&#252;", "u");
            myText = myText.Replace("&Uuml;", "u"); myText = myText.Replace("&#220;", "u");
            myText = myText.Replace("&yuml;", "ÿ"); myText = myText.Replace("&#255;", "ÿ");
            myText = myText.Replace("&#180;", "");
            myText = myText.Replace("&#96;", "");

            myText = myText.Replace("&nbsp", char.ConvertFromUtf32(160));
            myText = myText.Replace("&iexcl", char.ConvertFromUtf32(161));
            myText = myText.Replace("&cent", char.ConvertFromUtf32(162));
            myText = myText.Replace("&pound", char.ConvertFromUtf32(163));
            myText = myText.Replace("&curren", char.ConvertFromUtf32(164));
            myText = myText.Replace("&yen", char.ConvertFromUtf32(165));
            myText = myText.Replace("&brvbar", char.ConvertFromUtf32(166));
            myText = myText.Replace("&sect", char.ConvertFromUtf32(167));
            myText = myText.Replace("&uml", char.ConvertFromUtf32(168));
            myText = myText.Replace("&copy", char.ConvertFromUtf32(169));
            myText = myText.Replace("&ordf", char.ConvertFromUtf32(170));
            myText = myText.Replace("&laquo", char.ConvertFromUtf32(171));
            myText = myText.Replace("&not", char.ConvertFromUtf32(172));
            myText = myText.Replace("&shy", char.ConvertFromUtf32(173));
            myText = myText.Replace("&reg", char.ConvertFromUtf32(174));
            myText = myText.Replace("&macr", char.ConvertFromUtf32(175));
            myText = myText.Replace("&deg", char.ConvertFromUtf32(176));
            myText = myText.Replace("&plusmn", char.ConvertFromUtf32(177));
            myText = myText.Replace("&sup2", char.ConvertFromUtf32(178));
            myText = myText.Replace("&sup3", char.ConvertFromUtf32(179));
            myText = myText.Replace("&acute", char.ConvertFromUtf32(180));
            myText = myText.Replace("&micro", char.ConvertFromUtf32(181));
            myText = myText.Replace("&para", char.ConvertFromUtf32(182));
            myText = myText.Replace("&middot", char.ConvertFromUtf32(183));
            myText = myText.Replace("&cedil", char.ConvertFromUtf32(184));
            myText = myText.Replace("&sup1", char.ConvertFromUtf32(185));
            myText = myText.Replace("&ordm", char.ConvertFromUtf32(186));
            myText = myText.Replace("&raquo", char.ConvertFromUtf32(187));
            myText = myText.Replace("&frac14", char.ConvertFromUtf32(188));
            myText = myText.Replace("&frac12", char.ConvertFromUtf32(189));
            myText = myText.Replace("&frac34", char.ConvertFromUtf32(190));
            myText = myText.Replace("&iquest", char.ConvertFromUtf32(191));
            myText = myText.Replace("&Agrave", char.ConvertFromUtf32(192));
            myText = myText.Replace("&Aacute", char.ConvertFromUtf32(193));
            myText = myText.Replace("&Acirc", char.ConvertFromUtf32(194));
            myText = myText.Replace("&Atilde", char.ConvertFromUtf32(195));
            myText = myText.Replace("&Auml", char.ConvertFromUtf32(196));
            myText = myText.Replace("&Aring", char.ConvertFromUtf32(197));
            myText = myText.Replace("&AElig", char.ConvertFromUtf32(198));
            myText = myText.Replace("&Ccedil", char.ConvertFromUtf32(199));
            myText = myText.Replace("&Egrave", char.ConvertFromUtf32(200));
            myText = myText.Replace("&Eacute", char.ConvertFromUtf32(201));
            myText = myText.Replace("&Ecirc", char.ConvertFromUtf32(202));
            myText = myText.Replace("&Euml", char.ConvertFromUtf32(203));
            myText = myText.Replace("&Igrave", char.ConvertFromUtf32(204));
            myText = myText.Replace("&Iacute", char.ConvertFromUtf32(205));
            myText = myText.Replace("&Icirc", char.ConvertFromUtf32(206));
            myText = myText.Replace("&Iuml", char.ConvertFromUtf32(207));
            myText = myText.Replace("&ETH", char.ConvertFromUtf32(208));
            myText = myText.Replace("&Ntilde", char.ConvertFromUtf32(209));
            myText = myText.Replace("&Ograve", char.ConvertFromUtf32(210));
            myText = myText.Replace("&Oacute", char.ConvertFromUtf32(211));
            myText = myText.Replace("&Ocirc", char.ConvertFromUtf32(212));
            myText = myText.Replace("&Otilde", char.ConvertFromUtf32(213));
            myText = myText.Replace("&Ouml", char.ConvertFromUtf32(214));
            myText = myText.Replace("&times", char.ConvertFromUtf32(215));
            myText = myText.Replace("&Oslash", char.ConvertFromUtf32(216));
            myText = myText.Replace("&Ugrave", char.ConvertFromUtf32(217));
            myText = myText.Replace("&Uacute", char.ConvertFromUtf32(218));
            myText = myText.Replace("&Ucirc", char.ConvertFromUtf32(219));
            myText = myText.Replace("&Uuml", char.ConvertFromUtf32(220));
            myText = myText.Replace("&Yacute", char.ConvertFromUtf32(221));
            myText = myText.Replace("&THORN", char.ConvertFromUtf32(222));
            myText = myText.Replace("&szlig", char.ConvertFromUtf32(223));
            myText = myText.Replace("&agrave", char.ConvertFromUtf32(224));
            myText = myText.Replace("&aacute", char.ConvertFromUtf32(225));
            myText = myText.Replace("&acirc", char.ConvertFromUtf32(226));
            myText = myText.Replace("&atilde", char.ConvertFromUtf32(227));
            myText = myText.Replace("&auml", char.ConvertFromUtf32(228));
            myText = myText.Replace("&aring", char.ConvertFromUtf32(229));
            myText = myText.Replace("&aelig", char.ConvertFromUtf32(230));
            myText = myText.Replace("&ccedil", char.ConvertFromUtf32(231));
            myText = myText.Replace("&egrave", char.ConvertFromUtf32(232));
            myText = myText.Replace("&eacute", char.ConvertFromUtf32(233));
            myText = myText.Replace("&ecirc", char.ConvertFromUtf32(234));
            myText = myText.Replace("&euml", char.ConvertFromUtf32(235));
            myText = myText.Replace("&igrave", char.ConvertFromUtf32(236));
            myText = myText.Replace("&iacute", char.ConvertFromUtf32(237));
            myText = myText.Replace("&icirc", char.ConvertFromUtf32(238));
            myText = myText.Replace("&iuml", char.ConvertFromUtf32(239));
            myText = myText.Replace("&eth", char.ConvertFromUtf32(240));
            myText = myText.Replace("&ntilde", char.ConvertFromUtf32(241));
            myText = myText.Replace("&ograve", char.ConvertFromUtf32(242));
            myText = myText.Replace("&oacute", char.ConvertFromUtf32(243));
            myText = myText.Replace("&ocirc", char.ConvertFromUtf32(244));
            myText = myText.Replace("&otilde", char.ConvertFromUtf32(245));
            myText = myText.Replace("&ouml", char.ConvertFromUtf32(246));
            myText = myText.Replace("&divide", char.ConvertFromUtf32(247));
            myText = myText.Replace("&oslash", char.ConvertFromUtf32(248));
            myText = myText.Replace("&ugrave", char.ConvertFromUtf32(249));
            myText = myText.Replace("&uacute", char.ConvertFromUtf32(250));
            myText = myText.Replace("&ucirc", char.ConvertFromUtf32(251));
            myText = myText.Replace("&uuml", char.ConvertFromUtf32(252));
            myText = myText.Replace("&yacute", char.ConvertFromUtf32(253));
            myText = myText.Replace("&thorn", char.ConvertFromUtf32(254));
            myText = myText.Replace("&yuml", char.ConvertFromUtf32(255));
            myText = myText.Replace("&fnof", char.ConvertFromUtf32(402));
            myText = myText.Replace("&Alpha", char.ConvertFromUtf32(913));
            myText = myText.Replace("&Beta", char.ConvertFromUtf32(914));
            myText = myText.Replace("&Gamma", char.ConvertFromUtf32(915));
            myText = myText.Replace("&Delta", char.ConvertFromUtf32(916));
            myText = myText.Replace("&Epsilon", char.ConvertFromUtf32(917));
            myText = myText.Replace("&Zeta", char.ConvertFromUtf32(918));
            myText = myText.Replace("&Eta", char.ConvertFromUtf32(919));
            myText = myText.Replace("&Theta", char.ConvertFromUtf32(920));
            myText = myText.Replace("&Iota", char.ConvertFromUtf32(921));
            myText = myText.Replace("&Kappa", char.ConvertFromUtf32(922));
            myText = myText.Replace("&Lambda", char.ConvertFromUtf32(923));
            myText = myText.Replace("&Mu", char.ConvertFromUtf32(924));
            myText = myText.Replace("&Nu", char.ConvertFromUtf32(925));
            myText = myText.Replace("&Xi", char.ConvertFromUtf32(926));
            myText = myText.Replace("&Omicron", char.ConvertFromUtf32(927));
            myText = myText.Replace("&Pi", char.ConvertFromUtf32(928));
            myText = myText.Replace("&Rho", char.ConvertFromUtf32(929));
            myText = myText.Replace("&Sigma", char.ConvertFromUtf32(931));
            myText = myText.Replace("&Tau", char.ConvertFromUtf32(932));
            myText = myText.Replace("&Upsilon", char.ConvertFromUtf32(933));
            myText = myText.Replace("&Phi", char.ConvertFromUtf32(934));
            myText = myText.Replace("&Chi", char.ConvertFromUtf32(935));
            myText = myText.Replace("&Psi", char.ConvertFromUtf32(936));
            myText = myText.Replace("&Omega", char.ConvertFromUtf32(937));
            myText = myText.Replace("&alpha", char.ConvertFromUtf32(945));
            myText = myText.Replace("&beta", char.ConvertFromUtf32(946));
            myText = myText.Replace("&gamma", char.ConvertFromUtf32(947));
            myText = myText.Replace("&delta", char.ConvertFromUtf32(948));
            myText = myText.Replace("&epsilon", char.ConvertFromUtf32(949));
            myText = myText.Replace("&zeta", char.ConvertFromUtf32(950));
            myText = myText.Replace("&eta", char.ConvertFromUtf32(951));
            myText = myText.Replace("&theta", char.ConvertFromUtf32(952));
            myText = myText.Replace("&iota", char.ConvertFromUtf32(953));
            myText = myText.Replace("&kappa", char.ConvertFromUtf32(954));
            myText = myText.Replace("&lambda", char.ConvertFromUtf32(955));
            myText = myText.Replace("&mu", char.ConvertFromUtf32(956));
            myText = myText.Replace("&nu", char.ConvertFromUtf32(957));
            myText = myText.Replace("&xi", char.ConvertFromUtf32(958));
            myText = myText.Replace("&omicron", char.ConvertFromUtf32(959));
            myText = myText.Replace("&pi", char.ConvertFromUtf32(960));
            myText = myText.Replace("&rho", char.ConvertFromUtf32(961));
            myText = myText.Replace("&sigmaf", char.ConvertFromUtf32(962));
            myText = myText.Replace("&sigma", char.ConvertFromUtf32(963));
            myText = myText.Replace("&tau", char.ConvertFromUtf32(964));
            myText = myText.Replace("&upsilon", char.ConvertFromUtf32(965));
            myText = myText.Replace("&phi", char.ConvertFromUtf32(966));
            myText = myText.Replace("&chi", char.ConvertFromUtf32(967));
            myText = myText.Replace("&psi", char.ConvertFromUtf32(968));
            myText = myText.Replace("&omega", char.ConvertFromUtf32(969));
            myText = myText.Replace("&thetasym", char.ConvertFromUtf32(977));
            myText = myText.Replace("&upsih", char.ConvertFromUtf32(978));
            myText = myText.Replace("&piv", char.ConvertFromUtf32(982));
            myText = myText.Replace("&bull", char.ConvertFromUtf32(8226));
            myText = myText.Replace("&hellip", char.ConvertFromUtf32(8230));
            myText = myText.Replace("&prime", char.ConvertFromUtf32(8242));
            myText = myText.Replace("&Prime", char.ConvertFromUtf32(8243));
            myText = myText.Replace("&oline", char.ConvertFromUtf32(8254));
            myText = myText.Replace("&frasl", char.ConvertFromUtf32(8260));
            myText = myText.Replace("&weierp", char.ConvertFromUtf32(8472));
            myText = myText.Replace("&image", char.ConvertFromUtf32(8465));
            myText = myText.Replace("&real", char.ConvertFromUtf32(8476));
            myText = myText.Replace("&trade", char.ConvertFromUtf32(8482));
            myText = myText.Replace("&alefsym", char.ConvertFromUtf32(8501));
            myText = myText.Replace("&larr", char.ConvertFromUtf32(8592));
            myText = myText.Replace("&uarr", char.ConvertFromUtf32(8593));
            myText = myText.Replace("&rarr", char.ConvertFromUtf32(8594));
            myText = myText.Replace("&darr", char.ConvertFromUtf32(8595));
            myText = myText.Replace("&harr", char.ConvertFromUtf32(8596));
            myText = myText.Replace("&crarr", char.ConvertFromUtf32(8629));
            myText = myText.Replace("&lArr", char.ConvertFromUtf32(8656));
            myText = myText.Replace("&uArr", char.ConvertFromUtf32(8657));
            myText = myText.Replace("&rArr", char.ConvertFromUtf32(8658));
            myText = myText.Replace("&dArr", char.ConvertFromUtf32(8659));
            myText = myText.Replace("&hArr", char.ConvertFromUtf32(8660));
            myText = myText.Replace("&forall", char.ConvertFromUtf32(8704));
            myText = myText.Replace("&part", char.ConvertFromUtf32(8706));
            myText = myText.Replace("&exist", char.ConvertFromUtf32(8707));
            myText = myText.Replace("&empty", char.ConvertFromUtf32(8709));
            myText = myText.Replace("&nabla", char.ConvertFromUtf32(8711));
            myText = myText.Replace("&isin", char.ConvertFromUtf32(8712));
            myText = myText.Replace("&notin", char.ConvertFromUtf32(8713));
            myText = myText.Replace("&ni", char.ConvertFromUtf32(8715));
            myText = myText.Replace("&prod", char.ConvertFromUtf32(8719));
            myText = myText.Replace("&sum", char.ConvertFromUtf32(8721));
            myText = myText.Replace("&minus", char.ConvertFromUtf32(8722));
            myText = myText.Replace("&lowast", char.ConvertFromUtf32(8727));
            myText = myText.Replace("&radic", char.ConvertFromUtf32(8730));
            myText = myText.Replace("&prop", char.ConvertFromUtf32(8733));
            myText = myText.Replace("&infin", char.ConvertFromUtf32(8734));
            myText = myText.Replace("&ang", char.ConvertFromUtf32(8736));
            myText = myText.Replace("&and", char.ConvertFromUtf32(8743));
            myText = myText.Replace("&or", char.ConvertFromUtf32(8744));
            myText = myText.Replace("&cap", char.ConvertFromUtf32(8745));
            myText = myText.Replace("&cup", char.ConvertFromUtf32(8746));
            myText = myText.Replace("&int", char.ConvertFromUtf32(8747));
            myText = myText.Replace("&there4", char.ConvertFromUtf32(8756));
            myText = myText.Replace("&sim", char.ConvertFromUtf32(8764));
            myText = myText.Replace("&cong", char.ConvertFromUtf32(8773));
            myText = myText.Replace("&asymp", char.ConvertFromUtf32(8776));
            myText = myText.Replace("&ne", char.ConvertFromUtf32(8800));
            myText = myText.Replace("&equiv", char.ConvertFromUtf32(8801));
            myText = myText.Replace("&le", char.ConvertFromUtf32(8804));
            myText = myText.Replace("&ge", char.ConvertFromUtf32(8805));
            myText = myText.Replace("&sub", char.ConvertFromUtf32(8834));
            myText = myText.Replace("&sup", char.ConvertFromUtf32(8835));
            myText = myText.Replace("&nsub", char.ConvertFromUtf32(8836));
            myText = myText.Replace("&sube", char.ConvertFromUtf32(8838));
            myText = myText.Replace("&supe", char.ConvertFromUtf32(8839));
            myText = myText.Replace("&oplus", char.ConvertFromUtf32(8853));
            myText = myText.Replace("&otimes", char.ConvertFromUtf32(8855));
            myText = myText.Replace("&perp", char.ConvertFromUtf32(8869));
            myText = myText.Replace("&sdot", char.ConvertFromUtf32(8901));
            myText = myText.Replace("&lceil", char.ConvertFromUtf32(8968));
            myText = myText.Replace("&rceil", char.ConvertFromUtf32(8969));
            myText = myText.Replace("&lfloor", char.ConvertFromUtf32(8970));
            myText = myText.Replace("&rfloor", char.ConvertFromUtf32(8971));
            myText = myText.Replace("&lang", char.ConvertFromUtf32(9001));
            myText = myText.Replace("&rang", char.ConvertFromUtf32(9002));
            myText = myText.Replace("&loz", char.ConvertFromUtf32(9674));
            myText = myText.Replace("&spades", char.ConvertFromUtf32(9824));
            myText = myText.Replace("&clubs", char.ConvertFromUtf32(9827));
            myText = myText.Replace("&hearts", char.ConvertFromUtf32(9829));
            myText = myText.Replace("&diams", char.ConvertFromUtf32(9830));
            myText = myText.Replace("&OElig", char.ConvertFromUtf32(338));
            myText = myText.Replace("&oelig", char.ConvertFromUtf32(339));
            myText = myText.Replace("&Scaron", char.ConvertFromUtf32(352));
            myText = myText.Replace("&scaron", char.ConvertFromUtf32(353));
            myText = myText.Replace("&Yuml", char.ConvertFromUtf32(376));
            myText = myText.Replace("&circ", char.ConvertFromUtf32(710));
            myText = myText.Replace("&tilde", char.ConvertFromUtf32(732));
            myText = myText.Replace("&ensp", char.ConvertFromUtf32(8194));
            myText = myText.Replace("&emsp", char.ConvertFromUtf32(8195));
            myText = myText.Replace("&thinsp", char.ConvertFromUtf32(8201));
            myText = myText.Replace("&zwnj", char.ConvertFromUtf32(8204));
            myText = myText.Replace("&zwj", char.ConvertFromUtf32(8205));
            myText = myText.Replace("&lrm", char.ConvertFromUtf32(8206));
            myText = myText.Replace("&rlm", char.ConvertFromUtf32(8207));
            myText = myText.Replace("&ndash", char.ConvertFromUtf32(8211));
            myText = myText.Replace("&mdash", char.ConvertFromUtf32(8212));
            myText = myText.Replace("&lsquo", char.ConvertFromUtf32(8216));
            myText = myText.Replace("&rsquo", char.ConvertFromUtf32(8217));
            myText = myText.Replace("&sbquo", char.ConvertFromUtf32(8218));
            myText = myText.Replace("&ldquo", char.ConvertFromUtf32(8220));
            myText = myText.Replace("&rdquo", char.ConvertFromUtf32(8221));
            myText = myText.Replace("&bdquo", char.ConvertFromUtf32(8222));
            myText = myText.Replace("&dagger", char.ConvertFromUtf32(8224));
            myText = myText.Replace("&Dagger", char.ConvertFromUtf32(8225));
            myText = myText.Replace("&permil", char.ConvertFromUtf32(8240));
            myText = myText.Replace("&lsaquo", char.ConvertFromUtf32(8249));
            myText = myText.Replace("&rsaquo", char.ConvertFromUtf32(8250));
            myText = myText.Replace("&euro", char.ConvertFromUtf32(8364));
            myText = myText.Replace("      ", " ");
            myText = myText.Replace("     ", " ");
            myText = myText.Replace("    ", " ");
            myText = myText.Replace("   ", " ");
            myText = myText.Replace("  ", " ");

            myText = myText.Trim();
            return myText;
        }
    }
}