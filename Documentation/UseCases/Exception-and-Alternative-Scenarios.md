# Exception and Alternative Scenarios

## Authentication and authorization

| Scenario | Verified behavior |
|---|---|
| Unauthenticated request | Identity authorization prevents access or redirects/returns unauthorized. |
| Missing application session | Many project operations redirect to login or return 401/JSON error. |
| Missing/invalid unit | Claims transformation skips permission loading; movement actions reject invalid session unit. |
| Missing permission claim | Custom policy does not succeed; normal authorization failure applies. |
| Expired/invalid protected identifier | Decryption/Unprotect fails and action returns bad request/error JSON. |

## Project data alternatives

- New project versus edit: `ProjId == 0` invokes add; otherwise update.
- Ordinary versus special proposal: proposal IDs 5, 7, and 8 suppress/replace many fields.
- Submitted-but-not-processing edit: dates are locked and original date values are preserved.
- Re-vetted project: proposal ID 3 can generate a name and copy old attachments.
- Save versus submit: a saved project remains draft; submission requires all active required documents.
- Date change request: after submission, editable date-type changes create/update `DateApproval` and `LegacyHistory` unless `Remarks == "1"`.

## Forwarding exceptions

- Current PSM must exist, be addressed to the current session unit, be incomplete, active, and non-comment.
- Target unit is mandatory and cannot equal a CC unit.
- Forward date cannot precede project initiation date.
- Attachments must be non-empty PDF files <= 10 MB with a valid PDF signature.
- Whitelist side effects occur only for mapping IDs 78 and 88 and only when a matching row exists by project name.
- A failure after the current movement is completed could occur before attachment/CC persistence because the visible forward method does not wrap all writes in one transaction.

## Observation/pullback/undo alternatives

- Pullback creates a new movement with `IsPullBack = true` and returns it to the logged-in unit.
- Undo refuses stage 1; otherwise it completes the selected row and reopens a prior row.
- Missing previous movement returns a no-save result.
- Comment workflows can use project comments, stakeholder comments, or comment movements; the exact UI action depends on mapping data.

## Closure alternatives

- No latest active movement: close/reopen returns false.
- Existing foreclosure row: update it; otherwise insert one.
- Transaction failure: rollback and propagate.
- The visible code does not populate `ClosedByUserId` in the shown closure methods.

## Data and deployment gaps

1. Most current stored-procedure bodies are not checked in.
2. Complete status/action/unit mapping rows are not available in source.
3. The migration, SQL export, and current EF models are not identical in coverage; production schema should be treated as authoritative.
4. Some administrative CRUD routes rely on session checks rather than a clearly visible permission attribute.
5. Exact approval, rejection, observation, certificate, and whitelist business labels cannot be finalized without deployed master data.

## Operational recommendations for trace completion

Obtain a read-only schema/data extract containing procedure definitions, mapping/master rows, foreign keys, and current table definitions. Re-run each documented path against a non-production database and record the actual status/action descriptions and result sets before publishing a role-to-transition matrix.
