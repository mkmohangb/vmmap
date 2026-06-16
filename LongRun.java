public class LongRun {
    public static void main(String[] args) throws Exception {
        System.out.println("PID " + ProcessHandle.current().pid()
            + "   flags: " + java.lang.management.ManagementFactory
                              .getRuntimeMXBean().getInputArguments());
        System.out.println("Attach VMMap to this PID. Ctrl+C to exit.");
        Thread.sleep(Long.MAX_VALUE);
    }
}
