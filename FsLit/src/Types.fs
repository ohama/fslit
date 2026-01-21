module FsLit.Types

type TestCase = {
    Command: string
    Input: string option
    ExpectedOutput: string list
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

type TestResult =
    | Pass
    | Fail of CheckResult list
    | Error of message: string

type TestReport = {
    File: string
    Result: TestResult
}
