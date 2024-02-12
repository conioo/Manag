namespace Manag.Utilities
{
    internal static class ErrorDictionary
    {
        internal static List<(string, string)> Errors = new List<(string, string)>();

        static ErrorDictionary()
        {
            Errors.Add(("ERROR_DUP_NAME", "(0x34)You were not connected because a duplicate name exists on the network. If joining a domain, go to System in Control Panel to change the computer name and try again. If joining a workgroup, choose another workgroup name."));
            Errors.Add(("ERROR_NO_SPOOL_SPACE", "(0x3E)Space to store the file waiting to be printed is not available on the server."));
            Errors.Add(("ERROR_VERIFIER_STOP", "(0x219)Application verifier has found an error in the current process."));
            Errors.Add(("ERROR_TIMER_NOT_CANCELED", "(0x21D)An attempt was made to cancel or set a timer that has an associated APC and the subject thread is not the thread that originally set the timer with an associated APC routine."));
            Errors.Add(("ERROR_PORT_MESSAGE_TOO_LONG", "(0x222)Length of message passed to NtRequestPort or NtRequestWaitReplyPort was longer than the maximum message allowed by the port."));
            Errors.Add(("ERROR_INSTRUCTION_MISALIGNMENT", "(0x225)An attempt was made to execute an instruction at an unaligned address and the host system does not support unaligned instruction references."));
            Errors.Add(("ERROR_UNEXPECTED_MM_MAP_ERROR", "(0x22D)If an MM error is returned which is not defined in the standard FsRtl filter, it is converted to one of the following errors which is guaranteed to be in the filter. In this case information is lost, however, the filter correctly handles the exception."));
            Errors.Add(("ERROR_THREAD_NOT_IN_PROCESS", "(0x236)An attempt was made to operate on a thread within a specific process, but the thread specified is not in the process specified."));
            Errors.Add(("ERROR_UNRECOGNIZED_VOLUME", "(0x3ED)The volume does not contain a recognized file system. Please make sure that all required file system drivers are loaded and that the volume is not corrupted."));
            Errors.Add(("ERROR_REGISTRY_IO_FAILED", "(0x3F8)An I/O operation initiated by the registry failed unrecoverably. The registry could not read in, or write out, or flush, one of the files that contain the system's image of the registry."));
            Errors.Add(("ERROR_NO_UNICODE_TRANSLATION", "(0x459)No mapping for the Unicode character exists in the target multi-byte code page."));
            Errors.Add(("ERROR_INVALID_SERVER_STATE", "(0x548)The security account manager (SAM) or local security authority (LSA) server was in the wrong state to perform the security operation."));
            Errors.Add(("ERROR_MEMBERS_PRIMARY_GROUP", "(0x55E)The user cannot be removed from a group because the group is currently the user's primary group."));
            Errors.Add(("ERROR_LICENSE_QUOTA_EXCEEDED", "(0x573)The service being accessed is licensed for a particular number of connections. No more connections can be made to the service at this time because there are already as many connections as the service can accept."));
        }
    }
}