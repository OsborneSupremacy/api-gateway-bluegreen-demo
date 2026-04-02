# Deploy Green, Verify, Promote: Blue/Green Releases with API Gateway & Lambda

Presentation materials for the talk given at **AWS Community Day Midwest 2026**.

## Session Description

With API Gateway stages and Lambda aliases, you can deploy and test changes in production without impacting production workloads. In this talk, we'll walk through an opinionated blue/green workflow: deploy a 'green' API Gateway stage and 'green' Lambda version behind a separate URL, validate the changes, then promote green to blue via a control-plane switch for zero downtime—or don't promote if it doesn't look good.

## Viewing the Presentation

The presentation is built with [reveal.js](https://revealjs.com) and loads all dependencies from CDN. No build step is required.

1. Open `presentation/index.html` in a web browser (works from the file system or any static web server).
2. Use arrow keys or space bar to navigate slides.
3. Press `F` to enter fullscreen, `S` for speaker notes, `?` for all keyboard shortcuts.

### Running a local static server (optional)

```bash
# Using Python (no install required)
cd presentation
python3 -m http.server 8080
# Then open http://localhost:8080 in your browser
```

## Architecture Diagrams

Architectural diagrams in the presentation use placeholder boxes. To swap in the official AWS icons:

1. Download the AWS Architecture Icons from <https://aws.amazon.com/architecture/icons/>.
2. Place the SVG/PNG files in `presentation/img/`.
3. Replace the placeholder `<div class="aws-box …">` elements in `presentation/index.html` with `<img>` tags pointing to your icon files.

## Presentation Structure

| Slide | Topic |
|-------|-------|
| 1 | Title |
| 2 | Agenda |
| 3 | What is Blue/Green Deployment? |
| 4 | Why Blue/Green? |
| 5 | AWS Building Blocks (Lambda versions & aliases, API Gateway stages) |
| 6 | Lambda Versions & Aliases (deep dive) |
| 7 | API Gateway Stages (deep dive) |
| 8 | Architecture: Steady State (Blue production) |
| 9 | Workflow Overview |
| 10 | Step 1: Deploy Green |
| 11 | Step 2: Verify |
| 12 | Step 3a: Promote |
| 13 | Step 3b: Don't Promote / Rollback |
| 14 | Architecture: During Green Deployment |
| 15 | Architecture: After Promotion |
| 16 | Observability & Promotion Gates |
| 17 | IaC Considerations |
| 18 | Opinionated Workflow Summary |
| 19 | Common Pitfalls |
| 20 | Key Takeaways |
| 21 | Resources |
| 22 | Q&A / Closing |

## License

See [LICENSE](LICENSE).
