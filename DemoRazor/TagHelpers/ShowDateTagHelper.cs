using System;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DemoRazor.TagHelpers
{
    public class ShowDateTagHelper : TagHelper
    {
        private readonly HtmlEncoder _htmlEncoder;

        public ShowDateTagHelper(HtmlEncoder htmlEncoder)
        {
            _htmlEncoder = htmlEncoder;
        }

        [HtmlAttributeName("show-time")]
        public bool ShowTime { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            var sb = new StringBuilder();
            sb.Append(DateTime.Now.ToLongDateString());

            if (ShowTime)
            {
                sb.Append(' ');
                sb.Append(DateTime.Now.ToLongTimeString());
            }

            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
