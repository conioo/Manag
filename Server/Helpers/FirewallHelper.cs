using NetFwTypeLib;

namespace Server.Helpers
{
    internal static class FirewallHelper
    {
        internal static void AddFirewallRule(string name, string path, int port)
        {
            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

            firewallRule.Name = name;
            firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallRule.LocalPorts = port.ToString();
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallRule.ApplicationName = path;
            firewallRule.Enabled = true;

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            firewallPolicy.Rules.Add(firewallRule);
        }

        internal static void RemoveFirewallRule(string name)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            INetFwRule firewallRule = firewallPolicy.Rules.Item(name);

            if (firewallRule != null)
            {
                firewallPolicy.Rules.Remove(name);
            }
        }
    }
}
