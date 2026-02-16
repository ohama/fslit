module FsLit.Types

type TestCase = {
    Command: string
    Input: string option
    ExpectedOutput: string list
    ExpectedStderr: string list
    ExpectedExitCode: int option
    Timeout: int option
}

type TestFile = {
    Path: string
    Cases: TestCase list
}

type CheckResult =
    | Match
    | Mismatch of lineNum: int * expected: string * actual: string
    | MissingLine of lineNum: int * expected: string
    | ExtraOutput of lineNum: int * actual: string
    | ExitCodeMismatch of expected: int * actual: int
    | StderrMissing of expected: string
    | TimeoutExceeded of seconds: int

type TestResult =
    | Pass
    | Fail of errors: CheckResult list * actualStdout: string * actualStderr: string * actualExitCode: int
    | Error of message: string

type TestReport = {
    File: string
    Result: TestResult
}
