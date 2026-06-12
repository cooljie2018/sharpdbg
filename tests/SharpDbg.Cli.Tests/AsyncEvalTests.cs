using AwesomeAssertions;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using SharpDbg.Cli.Tests.Helpers;

namespace SharpDbg.Cli.Tests;

public class AsyncEvalTests(ITestOutputHelper testOutputHelper)
{
	[Fact]
	public async Task AsyncMethod_EvaluationRequest_Returns()
	{
		var startSuspended = false;

		var (debugProtocolHost, initializedEventTcs, debugEventTcs, adapter, p2) = TestHelper.GetRunningDebugProtocolHostInProc(testOutputHelper, startSuspended);
		using var _ = adapter;
		using var __ = new ProcessKiller(p2);
		using var ___ = debugProtocolHost;

		await debugProtocolHost
			.WithInitializeRequest()
			.WithAttachRequest(p2.Id)
			.WaitForInitializedEvent(initializedEventTcs);
		debugProtocolHost
			.WithBreakpointsRequest([10, 12], Path.JoinFromGitRoot("tests", "DebuggableConsoleApp", "AsyncMethodEvalClass.cs"))
			.WithConfigurationDoneRequest()
			.WithOptionalResumeRuntime(p2.Id, startSuspended);

		var stoppedEvent = await debugProtocolHost.WaitForStoppedEvent(debugEventTcs);
		debugProtocolHost
			.WithStackTraceRequest(stoppedEvent.ThreadId!.Value, out var stackTraceResponse)
			.WithScopesRequest(stackTraceResponse.StackFrames!.First().Id, out var scopesResponse);

		scopesResponse.Scopes.Should().HaveCount(1);
		var scope = scopesResponse.Scopes.Single();

		debugProtocolHost.WithVariablesRequest(scope.VariablesReference, out var variables);

		variables.Should().HaveCount(2);

		var stackFrameId = stackTraceResponse.StackFrames!.First().Id;
		debugProtocolHost.WithEvaluateRequest(stackFrameId, "localInt", out var evaluateResponse);
		evaluateResponse.Result.Should().Be("4");

	}
}
