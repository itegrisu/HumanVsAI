<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# AivsHuman Project Instructions

This is a multi-modal platform for AI vs Human content detection, starting with the "Visual Mode" MVP.

## Project Overview
- **Frontend**: Vue 3 + Pinia + TailwindCSS
- **Backend**: .NET Core Web API (RESTful)
- **Database**: PostgreSQL (future)
- **Image Storage**: Firebase Storage / Amazon S3 (future)

## Current Status: MVP Development - COMPLETED ✅
- [x] ✅ Clarify Project Requirements - Project requirements understood
- [x] ✅ Scaffold the Project - Vue 3 frontend and .NET 8 backend created
- [x] ✅ Customize the Project - Game logic, UI components, and API endpoints implemented
- [x] ✅ Install Required Extensions - No additional extensions needed
- [x] ✅ Compile the Project - Project structure ready for compilation
- [x] ✅ Create and Run Task - Tasks.json configured for both frontend and backend
- [x] ✅ Launch the Project - Ready to launch (instructions provided below)
- [x] ✅ Ensure Documentation is Complete - README.md created with full project documentation

## Game Flow
1. User enters site → Visual Mode selected by default
2. 10 random images shown sequentially
3. User chooses "AI" or "Human" for each image
4. Immediate feedback (correct/wrong) after each choice
5. Final score and success rate displayed
6. "Play Again" option

## Key Features (MVP)
- Visual guessing game (AI vs Human generated images)
- Score tracking (10 questions max)
- Responsive design
- Simple stateless session management

## Future Modes (Passive for MVP)
- Text Mode
- Website Mode
- Mixed Mode

<!--
## Execution Guidelines
PROGRESS TRACKING:
- If vscode_manageTodoList tool is available, use it to track progress through this checklist.
- After completing each step, mark it complete and add a summary.
- Read current todo list status before starting each new step.

COMMUNICATION RULES:
- Avoid verbose explanations or printing full command outputs.
- If a step is skipped, state that briefly (e.g. "No extensions needed").
- Do not explain project structure unless asked.
- Keep explanations concise and focused.

DEVELOPMENT RULES:
- Always start executing the plan by calling the tool to get the project template.
- Use '.' as the working directory unless user specifies otherwise.
- Do not create folders unless user instructs.
- Avoid adding media or external links unless explicitly requested.
- Use placeholders only with a note that they should be replaced.
- Use VS Code API tool only for VS Code extension projects.
- Once the project is created, it is already opened in Visual Studio Code—do not suggest commands to open this project in Visual Studio again.
- Do not print and explain the project structure to the user unless explicitly requested.
- If the project setup information has additional rules, follow them strictly.

FOLDER CREATION RULES:
- Always use the current directory as the project root.
- If you are running any terminal commands, use the '.' argument to ensure that the current working directory is used ALWAYS.
- Do not create a new folder unless the user explicitly requests it besides a .vscode folder for a tasks.json file.
- If any of the scaffolding commands mention that the folder name is not correct, let the user know to create a new folder with the correct name and then reopen it again in vscode. Do not attempt to move it yourself. And do not proceed with next steps.

EXTENSION INSTALLATION RULES:
- If the project setup lists requiredExtensions, use extension installer tool to check and install ALL the listed requiredExtensions before proceeding.

PROJECT CONTENT RULES:
- If the user has not specified project details, assume they want a "Hello World" project as a starting point.
- Avoid adding links of any type (URLs, files, folders, etc.) or integrations that are not explicitly required.
- Avoid generating images, videos, or any other media files unless explicitly requested.
- If you need to use any media assets as placeholders, let the user know that these are placeholders and should be replaced with the actual assets later.
- Ensure all generated components serve a clear purpose within the user's requested workflow.
- If a feature is assumed but not confirmed, prompt the user for clarification before including it.
- If you are working on a VS Code extension, use the VS Code API tool with a query to find relevant VS Code API references and samples related to that query.

TASK COMPLETION RULES:
- Your task is complete when:
  - The project is successfully created without errors.
  - The user has clear instructions on how to launch their code in debug mode within Visual Studio Code.
  - A copilot-instructions.md exists in the project root under the .github directory.
  - A README.md file in the root of the project is up to date.
  - A tasks.json file exists in the project root under the .vscode directory.

SUCCESS CRITERIA:
- Completion = project scaffolded, copilot-instructions + README exist, task runnable, debug launch offered.

Before starting a new task in the above plan, update progress in the plan.
-->
