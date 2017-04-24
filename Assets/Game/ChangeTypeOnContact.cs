﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeTypeOnContact : MonoBehaviour
{
    public HouseWell HouseWell;
    public ParticleSystem Splash;
    public AudioSource SplashSound;
    private bool alreadyTouchedWater;
    private List<GameObject> currentCollisions = new List<GameObject>();
    private float sinceTouch;
    private const float AttachDelay = 3f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!enabled) return;
        HouseWell.Remove(gameObject);
        currentCollisions.Add(other.gameObject);
        SetStatus("Touching");
    }

    private void SetStatus(string status)
    {
        gameObject.SetLayerRecoursively(status);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;
        if (alreadyTouchedWater) return;
        if (!other.gameObject.CompareTag("Water")) return;
        var splash = Instantiate(Splash);
        //splash.transform.SetParent(transform);
        splash.transform.position = transform.position;
        splash.Play();
        SplashSound.Play();
        HouseWell.Remove(gameObject);
        alreadyTouchedWater = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!enabled) return;
        currentCollisions.Remove(other.gameObject);
        SetStatus(currentCollisions.Any() ? "Touching" : "Flying");
    }

    private void Update()
    {
        if (!currentCollisions.Any())
        {
            sinceTouch = 0;
            return;
        }
        sinceTouch += Time.deltaTime;
        if (sinceTouch > AttachDelay)
        {
            var rb = GetComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            enabled = false;
        }
    }
}