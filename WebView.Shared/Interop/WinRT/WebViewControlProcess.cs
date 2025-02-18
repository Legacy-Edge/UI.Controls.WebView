// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using windows = Windows;

namespace Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT
{
    /// <summary>
    /// A proxy for <see cref="windows.Web.UI.Interop.WebViewControlProcess"/>.
    /// </summary>
    public sealed class WebViewControlProcess
    {
        [SecurityCritical]
        private readonly windows.Web.UI.Interop.WebViewControlProcess _process;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewControlProcess"/> class.
        /// </summary>
        public WebViewControlProcess()
            : this(new WebViewControlProcessOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewControlProcess"/> class with the specified <paramref name="processOptions"/>.
        /// </summary>
        /// <param name="processOptions">The process options.</param>
        public WebViewControlProcess(WebViewControlProcessOptions processOptions)
            : this(new windows.Web.UI.Interop.WebViewControlProcess(processOptions.ToWinRtWebViewControlProcessOptions()))
        {
        }

        private WebViewControlProcess(windows.Web.UI.Interop.WebViewControlProcess process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));
            SubscribeEvents();
        }

        /// <summary>
        /// Occurs when the underlying WWAHost process exits.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification ="This is the declaration from WinRT")]
        public event EventHandler<object> ProcessExited = (sender, args) => { };

        /// <summary>
        /// Gets the enterprise identifier.
        /// </summary>
        /// <value>The enterprise identifier.</value>
        public string EnterpriseId => _process.EnterpriseId;

        /// <summary>
        /// Gets a value indicating whether this instance is private network client server capability enabled.
        /// </summary>
        /// <value><c>true</c> if this instance can access the private network; otherwise, <c>false</c>.</value>
        public bool IsPrivateNetworkClientServerCapabilityEnabled => _process.IsPrivateNetworkClientServerCapabilityEnabled;

        /// <summary>
        /// Gets the process identifier (PID) of the underlying WWAHost.
        /// </summary>
        /// <value>The process identifier (PID).</value>
        public uint ProcessId => _process.ProcessId;

        /// <summary>
        /// Performs an implicit conversion from <see cref="windows.Web.UI.Interop.WebViewControlProcess"/> to <see cref="WebViewControlProcess"/>.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator WebViewControlProcess(windows.Web.UI.Interop.WebViewControlProcess process) => ToWebViewControlProcess(process);

        /// <summary>
        /// Creates a <see cref="WebViewControlProcess"/> from <see cref="windows.Web.UI.Interop.WebViewControlProcess"/>.
        /// </summary>
        /// <param name="process">The <see cref="windows.Web.UI.Interop.WebViewControlProcess"/> instance.</param>
        /// <returns><see cref="WebViewControlProcess"/></returns>
        public static WebViewControlProcess ToWebViewControlProcess(
            windows.Web.UI.Interop.WebViewControlProcess process) => new WebViewControlProcess(process);

        /// <summary>
        /// Terminates the underlying WWAHost process.
        /// </summary>
        public void Terminate() => _process.Terminate();

        internal async Task<WebViewControlHost> CreateWebViewControlHostAsync(IntPtr hostWindowHandle, Rect bounds)
        {
            if (hostWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(hostWindowHandle));
            }

            var wvc = await CreateWebViewControlAsync(hostWindowHandle, bounds);

            return new WebViewControlHost(wvc);
        }

        private IAsyncOperation<windows.Web.UI.Interop.WebViewControl> CreateWebViewControlAsync(
                    IntPtr hostWindowHandle,
                    Rect bounds)
        {
            var hwh = hostWindowHandle.ToInt64();
            return CreateWebViewControlAsync(hwh, bounds);
        }

        [SecurityCritical]
        private IAsyncOperation<windows.Web.UI.Interop.WebViewControl> CreateWebViewControlAsync(
            long hostWindowHandle,
            Rect bounds)
        {
            Security.DemandUnmanagedCode();
            if (hostWindowHandle == 0)
            {
                throw new ArgumentNullException(nameof(hostWindowHandle));
            }

            return _process.CreateWebViewControlAsync(hostWindowHandle, bounds);
        }

        private void OnWebViewControlProcessExited(windows.Web.UI.Interop.WebViewControlProcess sender, object args)
        {
            var handler = ProcessExited;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void SubscribeEvents()
        {
            _process.ProcessExited += OnWebViewControlProcessExited;
        }

        private void UnsubscribeEvents()
        {
            _process.ProcessExited -= OnWebViewControlProcessExited;
        }
    }
}