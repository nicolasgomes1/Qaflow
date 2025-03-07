import { test, expect, Page } from '@playwright/test';

const URL = 'https://localhost:7089';

test.beforeEach('Login User',async ({ page }) => {
    await page.goto(URL + '/Account/Login');
    await page.getByTestId('login_emailform').fill('admin@example.com');
    await page.getByTestId('login_passwordform').fill('Admin123!');
    await page.getByTestId('login_submitform').click();
    const heading = page.locator('strong', { hasText: 'Welcome, admin@example.com!'});
    await expect(heading).toBeVisible();
});

test.afterEach('Logout User',async ({ page }) => {
    await page.getByTestId('logout').hover();
    await page.getByTestId('logout').click();
    await new Promise(resolve => setTimeout(resolve, 500));
    const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
    await expect(guest_user).toBeVisible();
});

async function filterAndLaunchProject(page: Page, projectName = 'Demo Project Without Data') {
    await page.getByRole('columnheader', { name: 'Name filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill(projectName);
    await page.getByRole('button', { name: 'Apply' }).click();

    await page.getByRole('button', { name: 'launch' }).first().click({ force: true });
    await page.waitForLoadState('load');
}

/**
 * @param {Page} page - The Playwright page object.
 * @param {string} id - data-testid to locate the element.
 */
async function click_button(page: Page, id: string)
{
    const el = page.getByTestId(id);
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

async function submit_form(page: Page)
{
    const submitButton = page.getByTestId('submit');
    await submitButton.waitFor({ state: 'visible' });
    await submitButton.click({ force: true });
}


test('Create New Requirement', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);

    await filterAndLaunchProject(page);

    await click_button(page, 'd_requirements');
    
    await click_button(page, 'create_requirement');

    await fill_input(page, 'requirement_name', 'Test Requirement Playwright' + Random);

    await fill_input(page, 'requirement_description', 'Test Requirement Playwright Description' + Random);
    
    await select_dropdown_option(page, 'requirement_priority', 'Medium');
    

    await select_dropdown_option(page,'requirement_status', 'Completed');

    await select_dropdown_option(page,'requirement_assignedto', 'user@example.com');
    
    await submit_form(page);

    await validate_button(page, 'create_requirement');


    //delete the created requirement
    await page.getByRole('columnheader', { name: 'Name sort   filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill('Test Requirement Playwright' + Random);
    await page.getByRole('button', { name: 'Apply' }).click();
    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }
    await expect(page.getByRole('table')).toContainText('Test Requirement Playwright' + Random);

    await page.getByRole('button', { name: 'delete', exact: true }).click();
    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText('Test Requirement Playwright' + Random);
});