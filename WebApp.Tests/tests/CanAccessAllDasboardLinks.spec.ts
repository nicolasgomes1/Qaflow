import { test, expect, Page } from '@playwright/test';
import * as actions from '../TestSteps/ReusableTestSteps';
import * as login from '../TestSteps/LoginbyRole';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');
    const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
    await expect(guest_user).toBeVisible();
});

async function filterAndLaunchProject(page: Page, projectName = 'Demo Project Without Data') {
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill(projectName);
    await page.getByRole('button', { name: 'Apply' }).click();
    await page.waitForLoadState('networkidle');
    const el = page.getByRole('button', { name: 'launch' });
    await expect(el).toBeVisible();
    await el.hover();
    const tooltipSelector = '.rz-tooltip.rz-popup';
    if (await page.isVisible(tooltipSelector)) {
        await page.mouse.move(0, 0);
        await page.waitForTimeout(100);
        await page.waitForSelector(tooltipSelector, { state: 'hidden' });
    }
    await el.first().click({ force: true });
    await page.waitForLoadState('load');
}

test('Access Requirement Index', async ({ page })=> {
    test.slow();
    await filterAndLaunchProject(page);
    await actions.click_button(page, 'd_requirements');
    await actions.validate_button(page, 'create_requirement');
});

test('Access Test Cases Index', async ({ page })=> {
    test.slow();
    await filterAndLaunchProject(page);
    await actions.click_button(page, 'd_testcases');
    await actions.validate_button(page, 'create_testcase');
});

test('Access Test Plan Index', async ({ page })=> {
    test.slow();
    await filterAndLaunchProject(page);
    await actions.click_button(page, 'd_testplans');
    await actions.validate_button(page, 'create-testplan');
})

test('Access Test Execution Index', async ({ page })=> {
    test.slow();
    await filterAndLaunchProject(page);
    await actions.click_button(page, 'd_testexecutions');
    await actions.validate_button(page, 'create-testexecutions');
})

test('Access Bugs Index', async ({ page })=> {
    test.slow();
    await filterAndLaunchProject(page);
    await actions.click_button(page, 'd_bugs');
    await actions.validate_button(page, 'create-bug');
})

test('Access Reports Index', async ({ page })=> {
    test.slow();
    await filterAndLaunchProject(page);
    await actions.click_button(page, 'd_reports');
    await page.waitForLoadState('networkidle');
    const errorMessage = page.locator('.error-message');
    await expect(errorMessage).not.toBeVisible();
})

test('Access Milestones Index', async ({ page })=> {
    test.slow();
    await filterAndLaunchProject(page);
    await actions.click_button(page, 'd_milestones');
    await actions.validate_page_has_text(page, 'Test Execution Pass Rate');
})