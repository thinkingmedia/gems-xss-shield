﻿using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace XssShield
{
    /// <summary>
    /// Handles the work of traversing an HTML node tree.
    /// </summary>
    public class HtmlWalker
    {
        /// <summary>
        /// The original HTML
        /// </summary>
        private readonly string _document;

        /// <summary>
        /// The HTML node tree.
        /// </summary>
        private readonly HtmlDocument _html;

        /// <summary>
        /// The callback function for processing a node.
        /// </summary>
        /// <returns>True if the node was modified.</returns>
        public delegate void Process(Sanitized pResult, HtmlNode pNode);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pDocument">The source HTML</param>
        /// <param name="pEncoding">The encoding method</param>
        public HtmlWalker(string pDocument, Encoding pEncoding)
        {
            _document = pDocument;

            // NOTE: XSS attacks often rely upon readers that perform automatic closing of tags. If
            // you change any of these HtmlDocument settings. All unit tests must be verified again.
            _html = new HtmlDocument
                          {
                              OptionFixNestedTags = true, 
                              OptionAutoCloseOnEnd = true, 
                              OptionDefaultStreamEncoding = pEncoding, 
                              OptionUseIdAttribute = true, 
                              OptionWriteEmptyNodes = true
                          };
            _html.LoadHtml(pDocument);
        }

        /// <summary>
        /// Walks the current node and it's children.
        /// </summary>
        private static void Walk(HtmlNode pNode, Sanitized pResult, Process pCallback)
        {
            pCallback(pResult, pNode);
            pNode.ChildNodes.ToList().ForEach(pChild => Walk(pChild, pResult, pCallback));
        }

        /// <summary>
        /// Starts walking the HTML node tree. Calling the
        /// call back function for each node.
        /// </summary>
        /// <param name="pCallback">The callback function.</param>
        /// <returns>True if the DOM was modified.</returns>
        public Sanitized Execute(Process pCallback)
        {
            Sanitized result = new Sanitized();
            Walk(_html.DocumentNode, result, pCallback);

            return result;
        }
    }
}