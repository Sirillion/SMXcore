
namespace SMXcore
{
    public class XUiC_TargetBar : global::XUiC_TargetBar
    {
        private readonly CachedStringFormatter<int> statcurrentFormatterInt = new CachedStringFormatter<int>((int _i) => _i.ToString());

        public override void Update(float _dt)
        {
            viewComponent.IsVisible = (!(xui.playerUI.entityPlayer.AttachedToEntity != null) || !(xui.playerUI.entityPlayer.AttachedToEntity is EntityVehicle)) && !xui.playerUI.entityPlayer.IsDead();
            base.Update(_dt);

        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch(bindingName)
            {
                case "statmax":
                    value = Target != null ? statcurrentFormatterInt.Format((int)Target.Stats.Health.Max) : "";
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }
    }
}
