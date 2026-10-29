using Microsoft.AspNetCore.Components;

namespace ScottBush.Web.Content;

/// <summary>One job on the résumé. Bullets are markup so key numbers can be bold.</summary>
public sealed record Role(string Title, string Dates, MarkupString Organization, IReadOnlyList<MarkupString> Bullets);

public sealed record SkillGroup(string Name, string Items);

/// <summary>A certification or degree. Certifications link to the issuer's public verification page.</summary>
public sealed record Credential(string IssuedBy, string Name, string? VerifyUrl = null);

/// <summary>
/// The résumé, word for word. The web page and the printed PDF both render from this, so they
/// can't drift apart. Copy is final: only typography was normalized from the source résumé.
/// </summary>
public static class Resume
{
    public static readonly IReadOnlyList<Role> Roles =
    [
        new("Creator and sole developer, CardStock", "Mar 2026 – now",
            new("Independent project, <a href=\"https://cardstock.pro\">cardstock.pro</a>"),
            [
                new("Designed, built and operate cardstock.pro, a live market-data terminal for the Pokémon card aftermarket, developed end to end with Claude Code: .NET 10, Blazor WebAssembly, minimal APIs, EF Core and PostgreSQL, self-hosted on Linux behind a Cloudflare Tunnel with GitHub Actions CI and systemd deployment."),
                new("Built the companion scraping pipeline (PokemonInvestBatch) that has archived <b>10.3 million</b> price observations across <b>91,570</b> cards and <b>788</b> sets into an append-only, change-only PostgreSQL store where absent rows mean unchanged rather than missing, and designed the read layer around that distinction."),
                new("Enforced a hard read-only boundary between two applications sharing one database: separate schemas and migration lineages, least-privilege roles proven by testing denied writes against the live database, and EF Core view mappings locked in by model tests."),
                new("Specified <b>30</b> market indicators with per-metric data-sufficiency floors that render an explicit state instead of a number when unmet, governed by a <b>310-entry</b> decision ledger where every verified claim requires a receipt anyone can re-run."),
            ]),
        new("Lead Software Engineer", "Jun 2025 – Mar 2026",
            new("Farm Bureau Financial Services, West Des Moines"),
            [
                new("Re-platformed a business-critical nightly batch pipeline as the only developer on the project, syncing <b>22,000 to 150,000</b> life and annuity policies per night (ACORD TXLife) from the Verisk FAST system of record into local Microsoft SQL Server, moving from .NET Framework 2.0, WCF SOAP and ADO.NET to a .NET 8 gRPC service on Entity Framework Core."),
                new("Proved correctness by replaying production runs against the rewrite and diffing databases row for row before cutover."),
                new("Reduced per-record processing from roughly <b>150 ms to 30 ms</b> by consolidating dozens of database round trips per policy into two or three, cutting the nightly batch from <b>over 6 hours to 1–2 hours</b>."),
                new("Eliminated chronic risk to a hard 6 AM deadline that every downstream consumer depended on, including correspondence, general ledger feeds, CSR screens and compliance extracts."),
                new("Applied Claude Code to map hundreds of undocumented legacy fields, including direct mappings, ID and IDREF relation resolution and type-code lookups, reaching roughly <b>90 percent</b> accuracy up front and substantially de-risking the migration."),
                new("Provided production support for SSRS reports, diagnosing failures and handling deployments, and resolved a chronic timeout on a heavily used report by restructuring the underlying query with CTEs, restoring reliable on-demand execution."),
                new("Developed an ACORD schema validator that turned overnight payload rejections, 0 to 30 per night, from up to an hour of manual XML diagnosis into near-instant, routable findings."),
                new("Prototyped CQRS, Onion, N-Tier and Vertical Slice REPR architectures, drove team consensus on REPR with gRPC after benchmarks showed parity with Web API, and authored the standards playbook adopted as the team blueprint for greenfield .NET 8 development."),
            ]),
        new("Senior Software Engineer", "Apr 2022 – Apr 2025",
            new("Sertifi, Chicago (remote)"),
            [
                new("Developed the quorum-gated, multi-level e-signature workflow engine powering ordered signing on hundreds of thousands of hospitality agreements monthly, including per-client quorum rules, edit-triggered signature resets, turn recomputation on mid-flow contract changes, and stale-link handling that told signers exactly what changed."),
                new("Implemented a per-signature audit trail capturing IP address, login and event metadata, used by legal teams to resolve contract disputes."),
                new("Built signature placement on dynamic documents with permanent burn-in to finalized PDFs using Aspose and PDFTron."),
                new("Introduced Redis as the platform’s first distributed cache, scoped to the highest-traffic dashboard as a trial ground, using TTL-cached aggregates over multi-million-row tables and indefinitely cached immutable IDs to cut page load from roughly 10 to 4 seconds."),
                new("Owned the application side of decomposing the core “room” god table on live production using feature-flagged dual read and write paths, per-tenant legacy backfills, and overnight cutovers sequenced smallest tenant first across a multi-month rollout with <b>zero data loss</b>."),
                new("Removed chronic database blocking and a structural 8-signer cap as a result of that decomposition."),
                new("Created dead-letter-queue recovery tooling for a new Azure Service Bus payments pipeline that grew from 0 to <b>$150 million a quarter</b> in processed volume, with automated failure notifications carrying the full message payload for recovery."),
            ]),
        new("Software Developer II", "Jul 2021 – Apr 2022",
            new("Co-Op Financial, Des Moines"),
            [
                new("Developed MuleSoft Anypoint integration flows serving credit union API requests for member and transaction data across the payments network."),
                new("Contributed to the organization-wide Log4Shell response, auditing integration runtimes and dependencies, including precautionary log4net removal, during an industry-wide zero-day event at a payments company."),
            ]),
        new("Software Developer (contract)", "Feb 2021 – Jul 2021",
            new("Kuder, Adel"),
            [
                new("Maintained and extended a legacy web codebase in Lasso, a language obscure enough that no usable documentation existed, ramping up entirely by reading the code and authoring the standardization documents that made it maintainable for successors."),
            ]),
        new("Software Engineer", "Mar 2019 – Feb 2021",
            new("ESO Solutions, Austin (remote)"),
            [
                new("Designed and built a generic, reusable Excel ETL ingestion framework in C# using parameterized base classes that owned all parsing, type coercion and error handling, so onboarding a new entity or source system required only a source model and a mapper."),
                new("Reduced fire-department data migrations from competitor systems and the ESO legacy product from <b>months</b> of manual spreadsheet transformation to roughly <b>two weeks</b> of programming work."),
                new("Accumulated per-competitor mappers into a reusable library that made repeat onboardings largely configuration."),
                new("Implemented parsing against the ISO and OOXML Excel specifications to handle the many on-disk representations of identical-looking cells across producers."),
                new("Designed log-and-continue error handling so one bad row never killed a 20,000-row file, surfacing per-row failure reasons and live progress to an AngularJS UI through a polled status endpoint."),
                new("Routed validated rows over REST to owning bounded-context microservices."),
            ]),
        new("Software Developer", "Mar 2017 – Mar 2019",
            new("Community Choice Credit Union, Johnston"),
            [
                new("Built Lead Manager on a two-person team, owning every architectural decision behind a multi-tenant SaaS product live across <b>5</b> credit unions and still sold commercially today as SimplyFocused."),
                new("Designed a stage-based lead workflow state machine with a customer-facing visual progression tracker that let credit union members follow their own loan status, years before that experience was common in financial services."),
                new("Implemented optimistic concurrency on every lead update with token-checked writes and rejection of stale writes, paired with a SignalR live dashboard pushing real-time pool changes, so claimed leads disappeared from every other officer’s view instantly and double-claims were prevented by design."),
                new("Deployed on Azure managed services including Web Apps, Azure SQL with geo-replication failover groups, Traffic Manager and Blob Storage, deliberately inheriting Azure reliability SLAs rather than building custom redundancy a two-person team could not maintain."),
            ]),
    ];

    public static readonly IReadOnlyList<SkillGroup> Skills =
    [
        new("Languages", "C#, TypeScript, JavaScript, HTML, CSS, SQL, T-SQL, PowerShell"),
        new("Frameworks", ".NET 8 and 10, ASP.NET Core, Blazor, SignalR, REST APIs, gRPC, Protocol Buffers, Entity Framework Core, .NET Framework, xUnit"),
        new("Cloud and data", "Microsoft Azure (Service Bus, Web Apps, Azure SQL, Blob Storage, Traffic Manager), AWS, Microsoft SQL Server, PostgreSQL, Redis, IBM MQ"),
        new("Tools", "Claude Code, Azure DevOps, Git, GitHub, GitHub Actions, Visual Studio, JIRA, Aspose, PDFTron, MuleSoft Anypoint"),
        new("Core competencies", "System architecture, multi-tenant SaaS, legacy modernization, AI-assisted development, data integration, ETL and batch processing, Agile and Scrum, production support, requirements analysis"),
    ];

    public static readonly IReadOnlyList<Credential> Credentials =
    [
        new("Microsoft Certified, 2026", "Azure Developer Associate",
            "https://learn.microsoft.com/en-us/users/scottbush-2116/credentials/7c2cfe6f98cb81f7"),
        new("AWS Certified, 2026", "Developer – Associate",
            "https://www.credly.com/badges/ddf8796e-9828-48d2-a821-222d1136eedf"),
        new("Upper Iowa University, 2012", "BS, Management Information Systems"),
        new("DMACC, 2009", "AAS, IT and Network Administration"),
    ];
}
