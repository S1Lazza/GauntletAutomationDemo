// Fill out your copyright notice in the Description page of Project Settings.


#include "CustomGauntletController.h"
#include "ProfilingDebugging/CsvProfiler.h"
#include "TimerManager.h"
#include "Engine/World.h"
#include "Async/Async.h"

void UCustomGauntletController::OnPostMapChange(UWorld* World)
{
    UE_LOG(LogGauntlet, Display, TEXT("GauntletController started"));

    FTimerHandle dummy;
    GetWorld()->GetTimerManager().SetTimer(dummy, this, &UCustomGauntletController::StartTesting, SpinUpTime, false);
}

void UCustomGauntletController::StartTesting()
{
    //TODO: this is where you put your custom game code that should be run before profiling starts

    StartProfiling();
}

void UCustomGauntletController::StartProfiling()
{
    FCsvProfiler::Get()->BeginCapture();

    // set a timer for when profiling should end
    FTimerHandle dummy;
    GetWorld()->GetTimerManager().SetTimer(dummy, this, &UCustomGauntletController::StopProfiling, ProfilingTime, false);
}

void UCustomGauntletController::StopProfiling()
{
    UE_LOG(LogGauntlet, Display, TEXT("Stopping the profiler"));

    TSharedFuture<FString> future = FCsvProfiler::Get()->EndCapture();

    // launch an async task that polls the Future for completion
    // will in turn launch a task on the game thread once the CSV file is saved to disk
    AsyncTask(ENamedThreads::AnyThread, [this, future]()
        {
            while (!future.IsReady())
                FPlatformProcess::SleepNoStats(0);

            AsyncTask(ENamedThreads::GameThread, [this]()
                {
                    StopTesting();
                }
            );
        }
    );
}

void UCustomGauntletController::OnTick(float DeltaTime)
{
    //TODO: this is where you can put stuff that should happen on tick
}

void UCustomGauntletController::StopTesting()
{
    UE_LOG(LogGauntlet, Display, TEXT("GauntletController stopped"));
    EndTest(0);
}

