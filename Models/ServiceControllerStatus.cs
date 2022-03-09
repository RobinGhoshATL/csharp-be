using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace ProSuite.Support.WebAPI.Models
{
    public enum ServiceControllerStatus
    { //
        // Summary:
        //     The service is not running. This corresponds to the Win32 SERVICE_STOPPED constant,
        //     which is defined as 0x00000001.
        Stopped = 1,
        //
        // Summary:
        //     The service is starting. This corresponds to the Win32 SERVICE_START_PENDING
        //     constant, which is defined as 0x00000002.
        StartPending = 2,
        //
        // Summary:
        //     The service is stopping. This corresponds to the Win32 SERVICE_STOP_PENDING constant,
        //     which is defined as 0x00000003.
        StopPending = 3,
        //
        // Summary:
        //     The service is running. This corresponds to the Win32 SERVICE_RUNNING constant,
        //     which is defined as 0x00000004.
        Running = 4,
        //
        // Summary:
        //     The service continue is pending. This corresponds to the Win32 SERVICE_CONTINUE_PENDING
        //     constant, which is defined as 0x00000005.
        ContinuePending = 5,
        //
        // Summary:
        //     The service pause is pending. This corresponds to the Win32 SERVICE_PAUSE_PENDING
        //     constant, which is defined as 0x00000006.
        PausePending = 6,
        //
        // Summary:
        //     The service is paused. This corresponds to the Win32 SERVICE_PAUSED constant,
        //     which is defined as 0x00000007.
        Paused = 7
    }

    //
    // Summary:
    //     Indicates that a method defines an operation that is part of a service contract
    //     in a Windows Communication Foundation (WCF) application.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OperationContractAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.OperationContractAttribute
        //     class.
//        public OperationContractAttribute();

        //
        // Summary:
        //     Gets or sets the name of the operation.
        //
        // Returns:
        //     The name of the operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     System.ServiceModel.OperationContractAttribute.Name is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The value is an empty string.
        public string Name { get; set; }
        //
        // Summary:
        //     Gets or sets the WS-Addressing action of the request message.
        //
        // Returns:
        //     The action to use in generating the WS-Addressing Action header.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The value is null.
        public string Action { get; set; }
        //
        // Summary:
        //     Gets or sets a value that specifies whether the messages of an operation must
        //     be encrypted, signed, or both.
        //
        // Returns:
        //     One of the System.Net.Security.ProtectionLevel values. The default is System.Net.Security.ProtectionLevel.None.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The value is not one of the System.Net.Security.ProtectionLevel values.
        public ProtectionLevel ProtectionLevel { get; set; }
        //
        // Summary:
        //     Gets a value that indicates whether the messages for this operation must be encrypted,
        //     signed, or both.
        //
        // Returns:
        //     true if the System.ServiceModel.OperationContractAttribute.ProtectionLevel property
        //     is set to a value other than System.Net.Security.ProtectionLevel.None; otherwise,
        //     false. The default is false.
        public bool HasProtectionLevel { get; }
        //
        // Summary:
        //     Gets or sets the value of the SOAP action for the reply message of the operation.
        //
        // Returns:
        //     The value of the SOAP action for the reply message.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     System.ServiceModel.OperationContractAttribute.ReplyAction is null.
        public string ReplyAction { get; set; }
        //
        // Summary:
        //     Indicates that an operation is implemented asynchronously using a Begin<methodName>
        //     and End<methodName> method pair in a service contract.
        //
        // Returns:
        //     true if the Begin<methodName>method is matched by an End<methodName> method and
        //     can be treated by the infrastructure as an operation that is implemented as an
        //     asynchronous method pair on the service interface; otherwise, false. The default
        //     is false.
        public bool AsyncPattern { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether an operation returns a reply message.
        //
        // Returns:
        //     true if this method receives a request message and returns no reply message;
        //     otherwise, false. The default is false.
        public bool IsOneWay { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether the method implements an operation
        //     that can initiate a session on the server (if such a session exists).
        //
        // Returns:
        //     true if the operation is permitted to initiate a session on the server, otherwise,
        //     false. The default is true.
        public bool IsInitiating { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether the service operation causes the
        //     server to close the session after the reply message, if any, is sent.
        //
        // Returns:
        //     true if the operation causes the server to close the session, otherwise, false.
        //     The default is false.
        public bool IsTerminating { get; set; }
    }
}
