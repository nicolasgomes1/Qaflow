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

    await expect(page.getByTestId('d_requirements')).toBeVisible();
    await page.getByTestId('d_requirements').hover();
    await page.getByTestId('d_requirements').click({ force: true, delay: 100 });
    await page.waitForLoadState('load');
    await expect(page.getByTestId('create_requirement')).toBeVisible();

    await page.getByTestId('create_requirement').hover().then(() => {
        return page.getByTestId('create_requirement').click();
    });
    const req_name = "requirement_name";
    await page.getByTestId(req_name).hover()
    await page.getByTestId(req_name).click();
    await page.getByTestId(req_name).fill('Test Requirement Playwright' + Random);
    await page.getByTestId(req_name).press('Tab');
    await expect(page.getByTestId(req_name)).toHaveValue('Test Requirement Playwright' + Random);

    await page.getByTestId('requirement_description').hover()
    await page.getByTestId('requirement_description').click();
    await page.getByTestId('requirement_description').fill('Test Requirement Playwright Description' + Random);
    await page.getByTestId('requirement_description').press('Tab');
    await expect(page.getByTestId('requirement_description')).toHaveValue('Test Requirement Playwright Description' + Random);

    
    const req_priority = "requirement_priority";
    await page.getByTestId(req_priority).hover()
    await page.getByTestId(req_priority).click();
    await page.getByRole('option', { name: 'Medium' }).hover();
    await page.getByRole('option', { name: 'Medium' }).click();
    
    const req_status = "requirement_status";
    await page.getByTestId(req_status).hover()
    await page.getByTestId(req_status).click();
  //  await page.getByText('New').nth(1).click();
    await page.getByRole('option', { name: 'New' }).hover();
    await page.getByRole('option', { name: 'New' }).click();
    
    const req_assignedto = "requirement_assignedto";
    await page.getByTestId(req_assignedto).hover();
    await page.getByTestId(req_assignedto).click({force: true});
    await page.getByRole('option', { name: 'user@example.com' }).hover();
    await page.getByRole('option', { name: 'user@example.com' }).click({force: true});
    await page.getByTestId(req_assignedto).press('Tab');
    await page.getByTestId('submit').click({force:true});
    await page.waitForLoadState('load');
   // await expect(page.getByTestId('create_requirement')).toBeVisible();
});
