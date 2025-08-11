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
 * @param {string} id - data-testid to locate the element.
 * No tooltip needed
 */
async function click_element(page: Page, id: string) {
    const el = page.getByTestId(id).first();
    await el.waitFor({ state: 'visible' });
    await el.hover();

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

async function validate_button_disabled(page: Page, id: string) {
    const button = page.getByTestId(id).first();

    await expect(button).toBeDisabled({ timeout: 5000 });

    await page.waitForTimeout(100); // Give Blazor a moment to render final state
}

/**
 * @param {Page} page - The Playwright page object.
 * @param {string} id - data-testid to locate the element.
 * @param {string} value - value to be filled in the input element.
 */
async function fill_input(page: Page, id: string, value: string) {
    const maxTries = 3;
    let retryCount = 0;

    while (retryCount < maxTries) {
        try {
            const el = page.getByTestId(id);
            await el.waitFor({ state: 'visible', timeout: 5000 });

            // Force click to avoid hidden overlay issues
            await el.click({ force: true });

            // Clear the input before filling
            await el.fill('');
            await el.fill(value);

            // Wait briefly to ensure UI catches up
            await page.waitForTimeout(100);

            // Trigger blur by pressing Tab
            await el.press('Tab');

            // Double-check value
            await expect(el).toHaveValue(value, { timeout: 3000 });

            break; // Success
        } catch (error) {
            retryCount++;
            console.warn(`Attempt ${retryCount} failed to fill input with data-testid="${id}". Retrying...`);
            if (retryCount === maxTries) {
                console.error(`❌ Failed to fill input with data-testid="${id}" after ${maxTries} attempts.`);
                throw error; // Let the test fail
            }

            // Add short backoff to reduce retry flakiness
            await page.waitForTimeout(250);
        }
    }
}

async function check_checkbox(page: Page, id: string) {
    const checkbox = page.getByTestId(id);
    await checkbox.waitFor({ state: 'visible' });
    await checkbox.click();
}

async function fill_date_picker(page: Page, datatestid: string, value: string) {
    const maxTries = 3;
    let retryCount = 0;

    while (retryCount < maxTries) {
        try {
            // Find the input with id="StartDate" within the date picker container
            const dateInput = page.getByTestId(datatestid).locator('#StartDate');
            await dateInput.waitFor({ state: 'visible', timeout: 5000 });

            // Click to focus
            await dateInput.click({ force: true });

            // Clear existing value
            await dateInput.fill('');

            // Enter the new date
            await dateInput.fill(value);

            // Press Tab to trigger blur and date validation
            await dateInput.press('Tab');

            // Wait briefly for any date picker UI updates
            await page.waitForTimeout(100);

            break;
        } catch (error) {
            retryCount++;
            console.warn(`Attempt ${retryCount} failed to fill date picker with data-testid="${datatestid}". Retrying...`);
            if (retryCount === maxTries) {
                console.error(`❌ Failed to fill date picker with data-testid="${datatestid}" after ${maxTries} attempts.`);
                throw error;
            }
            await page.waitForTimeout(250);
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


    let Retries = 3;
    let RetrySubmit = 0;

    while (RetrySubmit < Retries) {
        try {
            await submitButton.click({ force: true });
            await expect(submitButton).toBeHidden({ timeout: 2000 });
            break;
        } catch (e) {
            RetrySubmit++;

            if (RetrySubmit === Retries) {
                console.error(`Failed to submit form after ${Retries} attempts. Error: ${e.message}`);
            }
        }
    }

    
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

    let InitialValue = 0;
    let Maxretries = 3;

    // 1. Open the filter menu by clicking the icon
    const filterIcon = page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i');
    await filterIcon.waitFor({ state: 'visible' });
    await filterIcon.hover();


    while (InitialValue < Maxretries) {
        try {
         //   console.log(`Attempt ${InitialValue}: To Filter`);

            await filterIcon.click({force: true});
            const overlayPanel = page.locator('.rz-overlaypanel:visible'); // Only consider visible elements
            await expect(overlayPanel).toBeVisible({timeout: 500});// Assert that the visible element is displayed

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

            await validate_button(page, 'd_requirements');
            break;
        } catch (e) {
            InitialValue++;
            if (InitialValue === Maxretries) {
                console.error(`Failed to validate button with data-testid="d_requirements" after ${Maxretries} attempts.`);
            }
        }
    }
}




async function UploadFile(page: Page, id: string) {
    const el = page.getByTestId(id);

    // Ensure the element is visible before interacting
    await el.waitFor({ state: 'visible' });

    // Click the element (if it's a button triggering file input)
    await el.click();

    // Locate the actual file input and set the file
    const fileInput = page.locator(`input[type="file"]`);
    const path = require('path'); // Import the 'path' module
    const filePath = path.resolve(__dirname, '../../WebApp.Tests/test_files/testfile.png'); // Construct the relative path
    await fileInput.setInputFiles(filePath);

    console.log(`File uploaded: ${filePath}`);
}




export { click_button, validate_button, fill_input, select_dropdown_option, submit_form, validate_input, validate_page_has_text, closeModal, LaunchProject, UploadFile, click_element, fill_date_picker, validate_button_disabled, check_checkbox };