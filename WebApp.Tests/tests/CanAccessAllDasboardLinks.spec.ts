import { test, expect, Page } from '@playwright/test';
import * as actions from '../TestSteps/ReusableTestSteps';
import * as login from '../TestSteps/LoginbyRole';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');

});

test('Access Requirement Specification Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_requirementsSpecification');
    await actions.validate_button(page, 'requirements_specification_create');
});

test('Access Requirement Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_requirements');
    await actions.validate_button(page, 'create_requirement');
});


test('Access Requirement Workflow Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_requirementsworkflow');
    await actions.validate_button(page, 'my_assignment');
    await actions.validate_button(page, 'all_assignment');

});


test('Access Test Cases Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_testcases');
    await actions.validate_button(page, 'create_testcase');
});

test('Access Test Cases Workflow Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_testcasesworkflow');
    await actions.validate_button(page, 'my_assignment');
    await actions.validate_button(page, 'all_assignment');
});

test('Access Test Plan Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_testplans');
    await actions.validate_button(page, 'create_testplan');
})

test('Access Test Plan Workflow Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_testplansworkflow');
    await actions.validate_button(page, 'my_assignment');
    await actions.validate_button(page, 'all_assignment');
})

test('Access Test Execution Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_testexecutions');
    await actions.validate_button(page, 'create-testexecutions');
})

test('Access Test Execution Workflow Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_testexecutionsworkflow');
    await actions.validate_button(page, 'my_assignment');
    await actions.validate_button(page, 'all_assignment');
})

test('Access Test Execution When Execution is Ready Index', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project With Data' );
    await actions.click_button(page, 'd_testexecutions');
    await actions.validate_button(page, 'create-testexecutions');
    await actions.click_button(page, 'create-testexecutions');

    const project_name = 'Test Execution Playwright' + Random;
    await actions.fill_input(page, 'testexecution_name', project_name);
    const project_description = 'Test Execution Description Playwright' + Random;
    await actions.fill_input(page, 'testexecution_description', project_description);
    await actions.select_dropdown_option(page, 'testexecution_priority', 'Medium');
    await actions.select_dropdown_option(page, 'testexecution_testplan', 'Test Plan Alpha');
    await actions.select_dropdown_option(page, 'testexecution_workflowstatus', 'Completed');

    await actions.select_dropdown_option(page,'testexecution_assignedto', 'user@example.com');
    await actions.submit_form(page);

    await actions.validate_button(page, 'create-testexecutions');
    await actions.click_button(page, 'logo');
    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }
    await actions.click_button(page, 'd_testexecutionsready');
    await actions.validate_page_has_text(page, 'Test Execution Ready!')
    
})

test('Access Bugs Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_bugs');
    await actions.validate_button(page, 'create_bug');
})

test('Access Reports Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_reports');
    await page.waitForLoadState('networkidle');
    const errorMessage = page.locator('.error-message');
    await expect(errorMessage).not.toBeVisible();
})

test('Access Milestones Index', async ({ page })=> {
    test.slow();
    await actions.LaunchProject(page, 'Demo Project Without Data' );
    await actions.click_button(page, 'd_milestones');
    await actions.validate_page_has_text(page, 'Test Execution Pass Rate');
})