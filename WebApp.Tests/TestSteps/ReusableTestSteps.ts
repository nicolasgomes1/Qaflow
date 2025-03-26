import { expect, Page } from '@playwright/test';

/**
 * @param {Page} page - The Playwright page object.
 * @param {string} id - data-testid to locate the element.
 */
async function click_button(page: Page, id: string)
{
    const el = page.getByTestId(id).first();
    await el.waitFor({ state: 'visible' });
    await el.hover();
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


export { click_button, validate_button, fill_input, select_dropdown_option, submit_form, validate_input, validate_page_has_text, closeModal };