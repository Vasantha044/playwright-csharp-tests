using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using System.Text.Json;

namespace PlaywrightTests.Utilities
{
    public class ActionHelper
    {
        private readonly IPage _page;

        public ActionHelper(IPage page)
        {
            _page = page;
        }

        // Check if an element is visible
        public async Task<bool> IsVisible(string selector)
        {
            var element = _page.Locator(selector);
            return await element.IsVisibleAsync();
        }

        // Check if an element is clickable (enabled)
        public async Task<bool> IsClickable(string selector)
        {
            var element = _page.Locator(selector);
            return await element.IsEnabledAsync();
        }

        // Click an element if it's visible and clickable
        public async Task Click(string selector)
        {
            if (await IsVisible(selector) && await IsClickable(selector))
            {
                Console.WriteLine($"Clicking on element: {selector}");
                await _page.ClickAsync(selector);
            }
            else
            {
                throw new Exception($"Element {selector} is not clickable");
            }
        }

        // Enter text into an input field
        public async Task SendKeys(string selector, string text)
        {
            if (await IsVisible(selector))
            {
                Console.WriteLine($"⌨️ Typing text '{text}' into {selector}");
                await _page.FillAsync(selector, text);
            }
            else
            {
                throw new Exception($"Cannot enter text, element {selector} is not visible");
            }
        }

        // Get text content of an element
        public async Task<string> GetText(string selector)
        {
            if (await IsVisible(selector))
            {
                return await _page.Locator(selector).InnerTextAsync();
            }
            throw new Exception($"Cannot get text, element {selector} is not visible");
        }

        // Get an attribute value of an element
        public async Task<string?> GetAttribute(string selector, string attribute)
        {
            if (await IsVisible(selector))
            {
                return await _page.Locator(selector).GetAttributeAsync(attribute);
            }
            throw new Exception($"Cannot get attribute '{attribute}', element {selector} is not visible");
        }

        // Hover over an element
        public async Task HoverOverElement(string selector)
        {
            if (await IsVisible(selector))
            {
                Console.WriteLine($" Hovering over element: {selector}");
                await _page.HoverAsync(selector);
            }
            else
            {
                throw new Exception($" Cannot hover, element {selector} is not visible");
            }
        }

        // Upload file(s) to a file input element
        public async Task UploadFile(string fileInputSelector, params string[] filePaths)
        {
            var input = _page.Locator(fileInputSelector);
            await input.SetInputFilesAsync(filePaths);
        }
    public async Task ScrollRightAsync(string selector)
{
    int pixels = 500;
    bool scrollToEnd = false;

    if (!string.IsNullOrEmpty(selector))
    {
        string safeSelector = JsonSerializer.Serialize(selector); // returns quoted, escaped string

        string script = scrollToEnd
            ? $"document.querySelector({safeSelector}).scrollLeft = document.querySelector({safeSelector}).scrollWidth;"
            : $"document.querySelector({safeSelector}).scrollLeft += {pixels};";

        await _page.EvaluateAsync(script);
    }
    else
    {
        string script = scrollToEnd
            ? "window.scrollTo(document.body.scrollWidth, 0);"
            : $"window.scrollBy({{ left: {pixels}, behavior: 'smooth' }});";

        await _page.EvaluateAsync(script);
    }
}

        
    }
}
