import { expect, Page } from '@playwright/test';

/**
 * @param {Page} page - The Playwright page object.
 * @param {string} id - data-testid to locate the element.
 */
async function click_button(page: Page, id: string) {
    const el = page.getByTestId(id).first();
    await el.waitFor({ state: 'visible' });
    await el.hover();

    // Wait for either tooltip to appear or timeout (if it never shows up)
    const tooltipSelector = '.rz-tooltip.rz-popup';
    try {
        await Promise.race([
            page.waitForSelector(tooltipSelector, { state: 'visible', timeout: 3000 }),
        ]);
    } catch (e) {
        // Ignore timeout errors
    }

    // Click the button even if the tooltip is visible
    await el.click({ force: true, delay: 100 });
    await page.waitForLoadState('networkidle');
}


/**
 * @param {Page} page - The Playwright page object.
 * @param {string} id - data-testid element to be validated.
 */
async function validate_button(page: Page, id: string)
{
    const el = page.getByTestId(id);
    await el.waitFor({ state: 'visible' });
    await page.waitForLoadState('networkidle');
}

/**
 * @param {Page} page - The Playwright page object.
 * @param {string} id - data-testid to locate the element.
 * @param {string} value - value to be filled in the input element.
 */
async function fill_input(page: Page, id: string, value: string)
{
    const el = page.getByTestId(id);
    await el.waitFor({ state: 'visible' });
    await el.click();
    await el.fill(value);
    await el.press('Tab');
    await expect(el).toHaveValue(value);
}

async function validate_input(page: Page, id: string, value: string)
{
    const el = page.getByTestId(id);
    await el.waitFor({ state: 'visible' });
    await expect(el).toHaveValue(value);
}

async function select_dropdown_option(page: Page, id: string, option: string)
{
    const el = page.getByTestId(id);
    await el.waitFor({ state: 'visible' });
    await el.click();
    const optionElement = page.getByRole('option', { name: option });
    await optionElement.waitFor({ state: 'visible' });
    await optionElement.click();
    await page.keyboard.press('Tab');
    await expect(el).toHaveText(option);
}

async function submit_form(page: Page, id: string = 'submit')
{
    const submitButton = page.getByTestId(id);
    await submitButton.waitFor({ state: 'visible' });
    await submitButton.click({ force: true });
}

async function validate_page_has_text(page: Page, text: string)
{
    expect(page.getByText(text)).toBeVisible();
}

async function closeModal(page: Page, dataTestId: string) {
    await page.evaluate((dataTestId) => {
        (function() {
            const selector = `[data-testid="${dataTestId}"]`; // Use the passed parameter

            const element = document.querySelector(selector);
            if (!element) {
                console.error(`Element with data-testid="${dataTestId}" not found.`);
                return;
            }

            // 1. Simulate Hover (to show the hand cursor)
            const mouseEnterEvent = new MouseEvent('mouseenter', {
                bubbles: true,
                cancelable: true,
                view: window
            });
            element.dispatchEvent(mouseEnterEvent);

            // 2. Simulate Click (mousedown + mouseup to trigger click)
            const mouseDownEvent = new MouseEvent('mousedown', {
                bubbles: true,
                cancelable: true,
                view: window
            });

            const mouseUpEvent = new MouseEvent('mouseup', {
                bubbles: true,
                cancelable: true,
                view: window
            });

            element.dispatchEvent(mouseDownEvent); // Simulate the mouse button being pressed
            element.dispatchEvent(mouseUpEvent);   // Simulate the mouse button being released

            // Optionally, trigger a final click event for browsers that require it
            const clickEvent = new MouseEvent('click', {
                bubbles: true,
                cancelable: true,
                view: window
            });
            element.dispatchEvent(clickEvent);
        })();
    }, dataTestId); // Pass the parameter to page.evaluate
}

async function LaunchProject(page: Page, project: string)
{
    // Hover and click the "Name sort filter_alt" icon to open the filter
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();

    // Fill in the project name in the filter textbox
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill(project);

    // Click the "Apply" button
    await page.getByRole('button', { name: 'Apply' }).click();

    // Wait for the network to idle (ensure the page loads and updates)
    await page.waitForLoadState('networkidle');

    // Wait for the tooltip or popup (rz-tooltip rz-popup) to disappear before continuing
    await page.waitForSelector('.rz-tooltip.rz-popup', { state: 'hidden', timeout: 5000 });

    // Wait for the tbody to have only one row after applying the filter
    await page.waitForFunction(() => {
        const tbody = document.querySelector('tbody');
        return tbody && tbody.rows.length === 1; // Wait until there is only one row in tbody
    }, { timeout: 5000 }); // Set a timeout for this check (5 seconds in this example)

    // Now click the "launch" button
    await page.getByRole('button', { name: 'launch' }).first().click({ force: true });

    // Wait for the page to load after the click
    await page.waitForLoadState('load');
}


async function UploadFile(page: Page, id: string) {
    const el = page.getByTestId(id);

    // Ensure the element is visible before interacting
    await el.waitFor({ state: 'visible' });

    // Click the element (if it's a button triggering file input)
    await el.click();

    // Locate the actual file input and set the file
    const fileInput = page.locator(`input[type="file"]`);
    const filePath = 'C:/Users/nicol/source/repos/WebApp/WebApp.Tests/test_files/testfile.png';
    await fileInput.setInputFiles(filePath);

    console.log(`File uploaded: ${filePath}`);
}

async function ClickTab(page: Page, TabTestId: string) {

    // Step 1: Unfocus any currently focused element to ensure we can interact freely
    await page.evaluate(() => document.body.focus());

    // Step 2: Locate the element by its data-testid
    const tab = page.locator(`[data-testid="${TabTestId}"]`);

    
// Step 3a: Wait for the tab to be attached to the DOM
    await tab.waitFor({state: 'attached'}).then(() => tab.scrollIntoViewIfNeeded());
    // Step 3: Scroll the element into view, even if it's hidden or off-screen

    // Step 4: Ensure the element is visible
    const isVisible = await tab.isVisible();
    if (!isVisible) {
        console.log(`Element with data-testid="${TabTestId}" is not visible, but proceeding to click anyway.`);
    }

    // Step 5: Force click the element
    await tab.click({ force: true });

}



export { click_button, validate_button, fill_input, select_dropdown_option, submit_form, validate_input, validate_page_has_text, closeModal, LaunchProject, UploadFile, ClickTab };