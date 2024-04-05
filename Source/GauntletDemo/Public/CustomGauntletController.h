// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GauntletTestController.h"
#include "CustomGauntletController.generated.h"

/**
 * 
 */
UCLASS()
class GAUNTLETDEMO_API UCustomGauntletController : public UGauntletTestController
{
	GENERATED_BODY()

private:
    // Time to wait after game start before doing anything.
    const float SpinUpTime = 3.f;

    // Time to run the profiler for.
    const float ProfilingTime = 7.f;

    UFUNCTION()
    void StartTesting();

    void StartProfiling();

    UFUNCTION()
    void StopProfiling();

    void StopTesting();

protected:
    virtual void OnTick(float DeltaTime) override;

public:
    virtual void OnPostMapChange(UWorld* World) override;
	
};
