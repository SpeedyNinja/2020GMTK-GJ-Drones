using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZScoreOutput
{
    public List<float> input;
    public List<int> signals;
    public List<float> avgFilter;
    public List<float> filtered_stddev;
}

public static class ZScore
{
    public static ZScoreOutput StartAlgo(List<float> input, int lag, float threshold, float influence)
    {
        // init variables!
        int[] signals = new int[input.Count];
        float[] filteredY = new List<float>(input).ToArray();
        float[] avgFilter = new float[input.Count];
        float[] stdFilter = new float[input.Count];

        var initialWindow = new List<float>(filteredY).Skip(0).Take(lag).ToList();

        avgFilter[lag - 1] = Mean(initialWindow);
        stdFilter[lag - 1] = StdDev(initialWindow);

        for (int i = lag; i < input.Count; i++)
        {
            if (Math.Abs(input[i] - avgFilter[i - 1]) > threshold * stdFilter[i - 1])
            {
                signals[i] = (input[i] > avgFilter[i - 1]) ? 1 : -1;
                filteredY[i] = influence * input[i] + (1 - influence) * filteredY[i - 1];
            }
            else
            {
                signals[i] = 0;
                filteredY[i] = input[i];
            }

            // Update rolling average and deviation
            var slidingWindow = new List<float>(filteredY).Skip(i - lag).Take(lag+1).ToList();

            var tmpMean = Mean(slidingWindow);
            var tmpStdDev = StdDev(slidingWindow);

            avgFilter[i] = Mean(slidingWindow);
            stdFilter[i] = StdDev(slidingWindow);
        }

        // Copy to convenience class 
        var result = new ZScoreOutput();
        result.input = input;
        result.avgFilter       = new List<float>(avgFilter);
        result.signals         = new List<int>(signals);
        result.filtered_stddev = new List<float>(stdFilter);

        return result;
    }

    private static float Mean(List<float> list)
    {
        // Simple helper function! 
        return list.Average();
    }

    private static float StdDev(List<float> values)
    {
        float ret = 0;
        if (values.Count() > 0)
        {
            float avg = values.Average();
            float sum = values.Sum(d => (float)Math.Pow(d - avg, 2));
            ret = (float)Math.Sqrt((sum) / (values.Count() - 1));
        }
        return ret;
    }
}
