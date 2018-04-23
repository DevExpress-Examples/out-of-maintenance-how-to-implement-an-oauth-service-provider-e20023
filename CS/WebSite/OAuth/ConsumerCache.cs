namespace OAuth_Provider_Basics.OAuth {
    using System;
    using DevExpress.Utils.OAuth.Provider;
    using DevExpress.Utils.OAuth;
    
    public class ConsumerCache : ConsumerStore {
        public override IConsumer GetConsumer(string consumerKey) {
            if (String.Equals(consumerKey, "anonymous", StringComparison.InvariantCulture)) {
                ConsumerBase consumer = new ConsumerBase();
                consumer.ConsumerKey = "anonymous";
                consumer.ConsumerSecret = "anonymous";
                return consumer;
            }
            return null;
        }
    }
}