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
async function validate_button1(page: Page, id: string)
{
    const el = page.getByTestId(id);
    await el.waitFor({ state: 'visible' });
    await page.waitForLoadState('networkidle');
}

async function validate_button(page: Page, id: string) {
    const button = page.getByTestId(id);

    await expect(button).toBeVisible({ timeout: 5000 });
    
    await page.waitForTimeout(100); // Give Blazor a moment to render final state
}

/**
 * @param {Page} page - The Playwright page object.
 * @param {string} id - data-testid to locate the element.
 * @param {string} value - value to be filled in the input element.
 */
async function fill_input(page: Page, id: string, value: string) {

    let MaxTries = 3;
    let RetryFill = 0;

    while (RetryFill < MaxTries) {
        try {
            console.log(`Attempt ${RetryFill + 1}: Filling input with data-testid="${id}"`);
            const el = page.getByTestId(id);
            await el.waitFor({state: 'visible'});
            await el.click();
            await el.fill(value);
            await el.press('Tab');
            await expect(el).toHaveValue(value);
            break;
        } catch (e) {
            RetryFill++;
            if (RetryFill === MaxTries) {
                console.error(`Failed to fill input with data-testid="${id}" after ${MaxTries} attempts.`);
            }
        }
    }
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

async function submit_form(page: Page) {
    const id = 'submit'
    const submitButton = page.getByTestId(id);

    // Ensure the button is visible and enabled
    await submitButton.waitFor({ state: 'visible' });
    await expect(submitButton).toBeEnabled();

    // Store the initial URL to detect a change (optional, used for debugging or logs)
    const initialUrl = page.url();

    // Click the submit button
    await submitButton.click({ force: true });
    await expect(submitButton).not.toBeVisible({timeout: 5000});
    
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

async function LaunchProject(page: Page, project: string) {
    // 1. Open the filter menu by clicking the icon
    const filterIcon = page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i');
    await filterIcon.waitFor({ state: 'visible' });
    await filterIcon.hover();
    await filterIcon.click({force: true});

    // 2. Fill in the filter textbox
    const filterInput = page.getByRole('textbox', { name: 'Name filter value' });
    await filterInput.waitFor({ state: 'visible' });
    await filterInput.fill(project);

    // 3. Click Apply
    const applyButton = page.getByRole('button', { name: 'Apply' });
    await applyButton.waitFor({ state: 'visible' });
    await applyButton.click();

    // 4. Wait for Blazor to finish rendering (tooltip gone, DOM settled)
    await page.waitForSelector('.rz-tooltip.rz-popup', { state: 'hidden', timeout: 5000 });
    await page.waitForTimeout(100); // Tiny delay to ensure Blazor rendering is complete

    // 5. Get the first visible row
    const visibleRow = page.locator('tbody tr:visible').first();
    await expect(visibleRow).toBeVisible({ timeout: 5000 });
    await expect(visibleRow).toContainText(project); // ✅ Less strict, just right

    const launchButton = visibleRow.getByRole('button', { name: 'launch' });
    await launchButton.waitFor({ state: 'visible' });
    await launchButton.click();



    // 7. Wait for navigation or success state
    await page.waitForLoadState('load');

    // 8. Confirm we landed on the right screen/component
    await validate_button(page, 'd_requirements');

    // Optional: add a selector to validate success feedback (toast, modal, etc.)
    // await page.waitForSelector('[data-testid="launch-success"]', { timeout: 5000 });
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
    // Step 1: Unfocus any currently focused element
    await page.evaluate(() => document.activeElement instanceof HTMLElement && document.activeElement.blur());

    // Step 2: Locate the element by its data-testid
    const tab = page.locator(`[data-testid="${TabTestId}"]`);

    // Step 3: Wait for the element to be attached to the DOM
    await tab.waitFor({ state: 'attached' });

    // Step 4: Scroll the element into view if needed
    await tab.scrollIntoViewIfNeeded();

    // Optional: wait for visibility (not required if using force, but helpful if you want more confidence)
    const isVisible = await tab.isVisible();
    if (!isVisible) {
        console.log(`Element with data-testid="${TabTestId}" is not visible, but proceeding to click anyway.`);
    }

    // Step 5: Force click the element
    await tab.click({ force: true });
}



export { click_button, validate_button, fill_input, select_dropdown_option, submit_form, validate_input, validate_page_has_text, closeModal, LaunchProject, UploadFile, ClickTab };