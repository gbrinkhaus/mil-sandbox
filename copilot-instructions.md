```instructions
# Copilot Agent Instructions

## Development Conventions

- **KISS principle:** Prefer simple, readable solutions. Avoid over-engineering.
- **No temp/security files in repo:** Never write temp or sensitive files to the repo tree.
- **Document function intent:** Use clear docstrings for all functions. Keep them concise and short, max 2 sentences.
- **Use Bootstrap & FontAwesome:** For all UI and icons.
- **MVC separation:** Routes = controller, services = logic, templates = view.
- **CSS/JS separation:** Put js + css in separate files, not inline in HTML templates.
- **PEP 8:** Follow throughout the project.
- **Cleancode:** Maintain consistent code style.

---

## CRITICAL: File Type Restrictions

- **NEVER edit scene files** (.tscn files) - EVER. Scene files are managed through the Godot editor UI only.
- **ONLY edit C# script files** (.cs files) - This is the only file type you should modify.
- All UI/layout changes must be done programmatically in C# scripts, never by editing scene files.
- If you think a scene file needs changes, suggest the required clicks to the user.

---

## Default Development Agent

You are a helpful coding assistant. Help with:
- Writing clean, maintainable code following conventions above
- Debugging issues
- Explaining concepts
- Code refactoring suggestions
- General development tasks

---

## Specialized Agents

The agents are located in separate files in the folder ./.github/agents/

When the user's message starts with one of these triggers, apply that agent's guidance:

- **Q:** - Apply code quality review from Q-quality.md (structure, testing, refactoring)
- **S:** - Apply security audit from S-security.md (vulnerabilities, compliance)
- **P:** - Apply performance analysis from P-performance.md (optimization, efficiency)
- **D:** - Apply deployment assessment from D-deployment.md (readiness, production)
- **W:** - Write a LinkedIN carousel about my findings regarding vibe coding

**Important:** When a trigger is detected, use ONLY that agent's specific guidance. Do not mix agent perspectives or revert to default behavior.

```