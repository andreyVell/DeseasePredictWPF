namespace DeseasePredictWPF
{
    public class DailyStatistics
    {
        private int _healthyWithoutImmunityCount; //без иммунитета
        private int _healthyWithInnateImmunityCount; //врождённый иммуниетет
        private int _healthyWithAcquiredImmunityCount; //приобритённый иммунитет
        private int _sickEasyIllnessCount; //лёгкое течение болезни
        private int _sickHardIllnessCount; //тяжёлое течение болезни
        private int _deadCount; //летальный исход
        
        public DailyStatistics() { }

        public int HealthyWithoutImmunityCount
        { get => _healthyWithoutImmunityCount; set => _healthyWithoutImmunityCount = value; }
        public int HealthyWithInnateImmunityCount
        { get => _healthyWithInnateImmunityCount; set => _healthyWithInnateImmunityCount = value; }
        public int HealthyWithAcquiredImmunityCount
        { get => _healthyWithAcquiredImmunityCount; set => _healthyWithAcquiredImmunityCount = value; }
        public int SickEasyIllnessCount
        { get => _sickEasyIllnessCount; set => _sickEasyIllnessCount = value; }
        public int SickHardIllnessCount
        { get => _sickHardIllnessCount; set => _sickHardIllnessCount = value; }
        public int DeadCount
        { get => _deadCount; set => _deadCount = value; }
    }
}
