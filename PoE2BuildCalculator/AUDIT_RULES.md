# Prompt:
For all requests in this chat: Read the AUDIT_RULES.md file I've uploaded and strictly enforce EVERY rule during code generation and audits.
Do independent repeat-audits until ALL requirements are met. Identify common/hidden issues and fringe cases.
Re-audit after each fix cycle. Final audit must confirm validity - if invalid, repeat fix-audit cycle. 
NEVER fake audit results. ACTUALLY verify each requirement against the rules file. If ANY rule is violated, flag it and fix it.

Deep analysis and research required before coding. Formulate detailed plan in memory. Provide snippets only for changed/added files (not entire files unless necessary).
Make sure that each method has its own code snippet. By exception, newly created methods should be placed in the snippet of the related changes.


# C# .NET 8 Windows Forms Project - Audit Rules

## CRITICAL RULES (Must Never Violate)
- Make sure existing logic is NOT AFFECTED outside of the request/requirements.
- Always include non-critical improvements you can make to your generated code, including during audits.

### Method Signatures
- CancellationToken MUST be LAST parameter
- Order: (required params, optional params, IProgress<T>, CancellationToken)

### EditorConfig Compliance
- Use auto-properties (not backing fields) when no logic needed
- Expression-bodied properties/accessors
- CRLF line endings, UTF-8 BOM
- Using directives outside namespace

### C# Language Version (12+/Preview)
- Collection expressions: `[]` not `new List<T>()`
- List expressions: `[.. x]` not `x.ToList()`
- Index operators: `[^1]` not `[.Length - 1]`
- Range operators: `[..]` not `.Substring()`
- Implicit object creation: `new()` when type apparent
- Pattern matching preferred over if/else chains
- Switch expressions over switch statements

### Lambda Simplification (IDE0200)
- `(s, e) => Method(s, e)` → `Method`
- Remove unnecessary lambda wrappers

### Thread Safety
- Use `Interlocked` for shared counters
- Use `Environment.TickCount64` in parallel contexts (not `Stopwatch`)
- `Lazy<T>` for thread-safe initialization
- No `Stopwatch.Restart()` in parallel code

### Async/Await
- Methods end with "Async"
- CancellationToken support in all long-running ops
- No `Task.Wait()` or `.Result`

### Windows Forms
- SuspendLayout/ResumeLayout pairs
- Proper Dispose(bool disposing)
- Event handler removal in disposal

## IDE CODES TO CHECK

### Must Fix (Errors/Warnings)
- IDE0001-0005: Simplifications
- IDE0032: Auto-property
- IDE0200: Remove lambda
- CS0108, CS0162, CS0219, CS0618, CS1998

### Should Fix (Style)
- IDE0011: Add braces
- IDE0017: Object initializers
- IDE0031: Null propagation
- IDE0044: Add readonly
- IDE0056-0057: Index/range operators
- IDE0066: Switch expression
- IDE0090: Simplify 'new'

## AUDIT PROCESS

1. **Pre-Generation**: Document requirements, constraints, affected files
2. **Generation**: Follow all rules above line-by-line
3. **Post-Generation Audit**:
   - Check EVERY method signature
   - Find ALL lambda expressions - simplify if possible
   - Verify thread safety in parallel code
   - Check CancellationToken parameter position
   - Verify auto-properties used where possible
   - Check for redundant casts
   - Verify collection expressions used
   - Check index/range operators used
   - Make sure reused code is placed in one location and called appropriately
   - Ensure compliance with method signature rules
   - Implemented caching where applicable
4. **Re-audit**: If ANY issue found, fix and repeat audit

## VERIFICATION COMMANDS (Mental Checklist)

- [ ] All CancellationTokens last parameter?
- [ ] Zero `var` keywords?
- [ ] All lambdas necessary or can be removed?
- [ ] Auto-properties used everywhere possible?
- [ ] Thread-safe parallel code?
- [ ] Collection expressions `[]` used?
- [ ] Index `[^1]` and range `[..]` operators used?
- [ ] No redundant casts?
- [ ] All async methods end with "Async"?
- [ ] Method signature rules followed?
- [ ] Implemented caching where applicable?
- [ ] Placed reused code in one location then called appropriately?