using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HybridRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float recoilDistance = 0.15f;      // Как далеко отлетают руки
    public float recoilAngle = 8f;             // Насколько задираются руки
    public float recoilDuration = 0.1f;        // Длительность отдачи
    public float returnSpeed = 5f;              // Скорость возврата
    
    [Header("References")]
    public XRGrabInteractable grabInteractable; // Оружие, которое держат
    
    private bool isRecoiling = false;
    private float recoilTimer = 0f;
    
    // Данные для каждой руки
    private class HandData
    {
        public Transform controller;
        public Vector3 originalPosition;
        public Quaternion originalRotation;
    }
    
    private HandData[] hands = new HandData[0];
    
    void Start()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();
    }
    
    public void Fire()
    {
        // Если уже отдача идет или нет оружия - выходим
        if (isRecoiling || grabInteractable == null) return;
        
        // Получаем все руки, которые держат оружие
        var interactors = grabInteractable.interactorsSelecting;
        if (interactors.Count == 0) return;
        
        // Создаем массив данных для рук
        hands = new HandData[interactors.Count];
        
        for (int i = 0; i < interactors.Count; i++)
        {
            var controllerTransform = interactors[i].transform;
            
            hands[i] = new HandData
            {
                controller = controllerTransform,
                originalPosition = controllerTransform.position,
                originalRotation = controllerTransform.rotation
            };
        }
        
        isRecoiling = true;
        recoilTimer = 0f;
        
        Debug.Log($"Recoil started on {hands.Length} hands");
    }
    
    void Update()
    {
        if (!isRecoiling || hands.Length == 0) return;
        
        recoilTimer += Time.deltaTime;
        
        // Проверяем, не отпустили ли оружие
        if (grabInteractable == null || grabInteractable.interactorsSelecting.Count == 0)
        {
            CancelRecoil();
            return;
        }
        
        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i]?.controller == null) continue;
            
            Transform ctrl = hands[i].controller;
            
            // Фаза 1: Отдача (0 - recoilDuration)
            if (recoilTimer <= recoilDuration)
            {
                float progress = recoilTimer / recoilDuration;
                
                // Плавно двигаем руку назад и вверх
                Vector3 recoilOffset = -ctrl.forward * recoilDistance + ctrl.up * (recoilDistance * 0.5f);
                Vector3 targetPos = hands[i].originalPosition + recoilOffset * (1f - progress);
                
                // Поворачиваем руку вверх
                Quaternion targetRot = hands[i].originalRotation * Quaternion.Euler(-recoilAngle * (1f - progress), 0, 0);
                
                ctrl.position = targetPos;
                ctrl.rotation = targetRot;
            }
            // Фаза 2: Возврат
            else
            {
                float returnProgress = (recoilTimer - recoilDuration) * returnSpeed;
                
                if (returnProgress >= 1f)
                {
                    // Полностью вернули
                    ctrl.position = hands[i].originalPosition;
                    ctrl.rotation = hands[i].originalRotation;
                }
                else
                {
                    // Плавно возвращаем
                    Vector3 recoilPos = hands[i].originalPosition - ctrl.forward * recoilDistance + ctrl.up * (recoilDistance * 0.5f);
                    Quaternion recoilRot = hands[i].originalRotation * Quaternion.Euler(-recoilAngle, 0, 0);
                    
                    ctrl.position = Vector3.Lerp(recoilPos, hands[i].originalPosition, returnProgress);
                    ctrl.rotation = Quaternion.Lerp(recoilRot, hands[i].originalRotation, returnProgress);
                }
            }
        }
        
        // Если все руки вернулись - выключаем отдачу
        if (recoilTimer > recoilDuration + (1f / returnSpeed))
        {
            isRecoiling = false;
            hands = new HandData[0];
        }
    }
    
    public void CancelRecoil()
    {
        if (!isRecoiling || hands.Length == 0) return;
        
        // Возвращаем все руки в исходное положение
        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i]?.controller != null)
            {
                hands[i].controller.position = hands[i].originalPosition;
                hands[i].controller.rotation = hands[i].originalRotation;
            }
        }
        
        isRecoiling = false;
        hands = new HandData[0];
    }
    
    void OnDisable()
    {
        CancelRecoil();
    }
}