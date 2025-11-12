using System;
using System.Collections;
using UnityEngine;

namespace DamageDealers
{
    public class LaserDamageDealer : SimpleDamageDealer
    {
        [SerializeField] private bool instantSpawn;
        [SerializeField] private float timeBeforeActivation;
        private float _targetedWidth;
        private LineRenderer _line;
        private EdgeCollider2D _lineCollider;
        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _lineCollider = GetComponent<EdgeCollider2D>();
        
            _targetedWidth = _line.startWidth;
            _lineCollider.edgeRadius = _targetedWidth / 2f;
        }

        IEnumerator Start()
        {
            if (!instantSpawn)
            {
                _line.startWidth = 0.1f;
                _line.endWidth = 0.1f;
                ChangeAlphaKeys(0.1f);

                yield return new WaitForSeconds(timeBeforeActivation);
        
                //Activate in 1 second
                float tempTimer = 0;
                while (_line.startWidth < _targetedWidth)
                {
                    float widthTempValue = Mathf.Lerp(0.1f, _targetedWidth, tempTimer);
                    _line.startWidth = widthTempValue;
                    _line.endWidth = widthTempValue;
            
                    float alphaTempValue = Mathf.Lerp(0.1f, 1f, tempTimer);
                    ChangeAlphaKeys(alphaTempValue);
            
                    yield return new WaitForEndOfFrame();
                    tempTimer = Math.Clamp(tempTimer + Time.deltaTime, 0f,1f);
                }
            }
        
            _lineCollider.enabled = true;
            while (gameObject)
            {
                _lineCollider.points = new Vector2[] {
                    (_line.GetPosition(0) - gameObject.transform.position) /2f,
                    (_line.GetPosition(1) - gameObject.transform.position) / 2f
                };
                yield return new WaitForEndOfFrame();
            }
        }


        void ChangeAlphaKeys(float newValue)
        {
            Gradient tempColorGradient = new Gradient();
            GradientAlphaKey[] tempAlphaKeys = {new(newValue, 0), new(newValue, 1)};
            tempColorGradient.SetKeys(_line.colorGradient.colorKeys,tempAlphaKeys);
            _line.colorGradient = tempColorGradient;
        }
    }
}
