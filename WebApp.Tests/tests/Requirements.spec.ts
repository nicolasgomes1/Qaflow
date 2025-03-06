import { test, expect } from '@playwright/test';

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

async function filterAndLaunchProject(page, projectName = 'Demo Project Without Data') {
    await page.getByRole('columnheader', { name: 'Name filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill(projectName);
    await page.getByRole('button', { name: 'Apply' }).click();

    await page.getByRole('button', { name: 'launch' }).first().click({ force: true });
    await page.waitForLoadState('load');
}


test('Create New Requirement', async ({ page })=> {
    const Random = Math.floor(Math.random() * 1000);

    await filterAndLaunchProject(page);

    const requirements = page.getByTestId('d_requirements');
    await requirements.waitFor({ state: 'visible' });
    await requirements.hover();
    await requirements.click({ force: true, delay: 100 });
    await page.waitForLoadState('networkidle');

    const createRequirement = page.getByTestId('create_requirement');
    await createRequirement.waitFor({ state: 'visible' });
    await createRequirement.hover();
    await createRequirement.click();

    const reqName = page.getByTestId('requirement_name');
    await reqName.waitFor({ state: 'visible' });
    await reqName.click();
    await reqName.fill('Test Requirement Playwright' + Random);
    await reqName.press('Tab');
    await expect(reqName).toHaveValue('Test Requirement Playwright' + Random);

    const reqDescription = page.getByTestId('requirement_description');
    await reqDescription.waitFor({ state: 'visible' });
    await reqDescription.click();
    await reqDescription.fill('Test Requirement Playwright Description' + Random);
    await reqDescription.press('Tab');
    await expect(reqDescription).toHaveValue('Test Requirement Playwright Description' + Random);

    const reqPriority = page.getByTestId('requirement_priority');
    await reqPriority.waitFor({ state: 'visible' });
    await reqPriority.click();

    const mediumOption = page.getByRole('option', { name: 'Medium' });
    await mediumOption.waitFor({ state: 'visible' });
    await mediumOption.click();
    await page.keyboard.press('Tab');

    const reqStatus = page.getByTestId('requirement_status');
    await reqStatus.waitFor({ state: 'visible' });
    await reqStatus.click();

    const completedOption = page.getByRole('option', { name: 'Completed' });
    await completedOption.waitFor({ state: 'visible' });
    await completedOption.click();
    await page.keyboard.press('Tab');

    const reqAssignedTo = page.getByTestId('requirement_assignedto');
    await reqAssignedTo.waitFor({ state: 'visible' });
    await reqAssignedTo.click();

    const userOption = page.getByRole('option', { name: 'user@example.com' });
    await userOption.waitFor({ state: 'visible' });
    await userOption.click();
    await reqAssignedTo.press('Tab');

    const submitButton = page.getByTestId('submit');
    await submitButton.waitFor({ state: 'visible' });
    await submitButton.click({ force: true });
    await createRequirement.waitFor({ state: 'visible' });
    await page.waitForLoadState('networkidle');
    await page.getByRole('columnheader', { name: 'Name sort   filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill('Test Requirement Playwright' + Random);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.getByRole('table')).toContainText('Test Requirement Playwright' + Random);
    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }
    await page.getByRole('button', { name: 'delete', exact: true }).click();
    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText('Test Requirement Playwright' + Random);
});